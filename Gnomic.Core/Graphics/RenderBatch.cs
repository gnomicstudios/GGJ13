using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Gnomic.Graphics
{    
    /// <summary>
    /// Inspired by SpriteBatch, this class attempts to reduce DrawUserPrimitive calls
    /// It also groups draw calls by texture to reduce texture changes. Be sure to call
    /// Reset() at the beginning or end of a frame.
    /// </summary>
    public class RenderBatch
    {
        /// <summary>
        /// There will be one BatchList per texture to be rendered
        /// </summary>
        public class BatchList
        {
            internal List<VertexPositionColorTexture[]> batches = new List<VertexPositionColorTexture[]>(16);
            internal int batchCurrent = -1;
            internal int batchPos = 0;
            internal Texture2D texture;
        }

        GraphicsDevice device;
        Dictionary<Texture2D, BatchList> textureBatches = new Dictionary<Texture2D, BatchList>();
        Dictionary<Texture2D, BatchList> textureStripBatches = new Dictionary<Texture2D, BatchList>();

        public List<BatchList> batchesToRender = new List<BatchList>();

        int maxPrimitivesPerBatch;
        public object Tag;
        public Effect Effect;
        IEffectMatrices fxMatrices;
        EffectParameter fxParamTexture;

        bool hasTriangles;
        public bool HasTriangles
        {
            get { return hasTriangles; }
        }

        public RenderBatch(GraphicsDevice device)
            : this(device, 200, true)
        {}

        public RenderBatch(GraphicsDevice device, int primitivesPerBatch, bool enableAlphaBlend)
        {
            this.device = device;
            if (enableAlphaBlend)
            {
                BasicEffect be = new BasicEffect(device);
                Effect = be;
                fxMatrices = be;
                be.TextureEnabled = true;
                be.VertexColorEnabled = true;
                be.LightingEnabled = false;
                fxParamTexture = Effect.Parameters["Texture"];
            }
            else
            {
                AlphaTestEffect ate = new AlphaTestEffect(device);
                Effect = ate;
                fxMatrices = ate;
                ate.VertexColorEnabled = true;
                ate.ReferenceAlpha = 128;
                fxParamTexture = Effect.Parameters["Texture"];
            }

            this.maxPrimitivesPerBatch = primitivesPerBatch;
        }

        public void RegisterTexture(Texture2D texture)
        {
            if (texture != null && !textureBatches.ContainsKey(texture))
            {
                BatchList batches = new BatchList();
                batches.texture = texture;
                batches.batches.Add(new VertexPositionColorTexture[maxPrimitivesPerBatch * 3]);
                batches.batchCurrent = -1;
                textureBatches.Add(texture, batches);
                batchesToRender.Add(batches);
            }
        }

        public void MoveTextureToEnd(Texture2D texture)
        {
            if (texture != null && textureBatches.ContainsKey(texture))
            {
                BatchList batches = textureBatches[texture];
                batchesToRender.Remove(batches);
                batchesToRender.Add(batches);
            }
        }
        
        public void AddGeometry(VertexPositionColorTexture[] verts, Texture2D texture)
        {
            AddGeometry(verts, texture, null, 0, verts.Length);
        }
        public void AddGeometry(VertexPositionColorTexture[] verts, Texture2D texture, Matrix? transform)
        {
            AddGeometry(verts, texture, transform, 0, verts.Length);
        }

        public void AddGeometry(VertexPositionColorTexture[] verts, Texture2D texture, int startIndex, int amountToCopy)
        {
            AddGeometry(verts, texture, null, startIndex, amountToCopy);
        }

        public void AddGeometry(VertexPositionColorTexture[] verts, Texture2D texture, Matrix? world, int startIndex, int amountToCopy)
        {
            if (verts == null)
                throw new ArgumentNullException("RenderBatch.AddGeometry requries non-null vertices array");

            int maxVertsPerBatch = maxPrimitivesPerBatch * 3;

            BatchList batches;
            if (!textureBatches.ContainsKey(texture))
            {
                batches = new BatchList();
                batches.texture = texture;
                batches.batches.Add(new VertexPositionColorTexture[maxVertsPerBatch]);
                batches.batchCurrent = 0;
                textureBatches.Add(texture, batches);
                batchesToRender.Add(batches);

                System.Diagnostics.Debug.WriteLine("Adding new texture to renderbatch");
            }
            else
            {
                batches = textureBatches[texture];
                if (batches.batchCurrent < 0)
                {
                    batches.batchCurrent = 0;
                    if (batches.batches.Count == 0)
                    {
                        batches.batches.Add(new VertexPositionColorTexture[maxVertsPerBatch]);

                        System.Diagnostics.Debug.WriteLine("Adding batch to renderbatch (1)");
                    }
                }
            }

            VertexPositionColorTexture[] batch = batches.batches[batches.batchCurrent];

            int vertsLeft = amountToCopy;
            int vertPos = startIndex;

            // Add over multiple batches if needed
            if (vertsLeft > maxVertsPerBatch - batches.batchPos)
            {
                while (vertsLeft > maxVertsPerBatch - batches.batchPos)
                {
                    // Use up the curent batch and add a new one
                    int count = maxVertsPerBatch - batches.batchPos;
                    CopyTransformed(verts, vertPos, batch, batches.batchPos, count, world);
                    
                    vertPos += count;
                    vertsLeft -= count;

                    if (batches.batchCurrent == batches.batches.Count - 1)
                    {
                        batches.batches.Add(new VertexPositionColorTexture[maxVertsPerBatch]);

                        System.Diagnostics.Debug.WriteLine("Adding batch to renderbatch ({0})", batches.batches.Count);
                    }
                    batches.batchCurrent++;
                    batch = batches.batches[batches.batchCurrent];
                    batches.batchPos = 0;
                }
            }

            // Copy remaining verts to the current batch
            CopyTransformed(verts, vertPos, batch, batches.batchPos, vertsLeft, world);

            batches.batchPos += vertsLeft;

            hasTriangles = true;
        }

        public void AddGeometryTriStrip(VertexPositionColorTexture[] verts, int numVerts, Texture2D texture, Matrix? world)
        {
            if (verts == null)
                throw new ArgumentNullException("RenderBatch.AddGeometryTriStrip requries non-null vertices array");

            int maxVertsPerBatch = maxPrimitivesPerBatch * 3;

            BatchList batches;
            if (!textureStripBatches.ContainsKey(texture))
            {
                batches = new BatchList();
                batches.texture = texture;
                batches.batches.Add(new VertexPositionColorTexture[maxVertsPerBatch]);
                batches.batchCurrent = 0;
                textureStripBatches.Add(texture, batches);
                batchesToRender.Add(batches);
            }
            else
            {
                batches = textureStripBatches[texture];
                if (batches.batchCurrent < 0)
                {
                    batches.batchCurrent = 0;
                    if (batches.batches.Count == 0)
                        batches.batches.Add(new VertexPositionColorTexture[maxVertsPerBatch]);
                }
            }

            VertexPositionColorTexture[] batch = batches.batches[batches.batchCurrent];

            int vertsLeft = numVerts;
            int vertPos = 0;

            // Add over multiple batches if needed
            if (vertsLeft > maxVertsPerBatch - batches.batchPos)
            {
                while (vertsLeft > maxVertsPerBatch - batches.batchPos)
                {
                    // Use up the curent batch and add a new one
                    int count = maxVertsPerBatch - batches.batchPos;
                    CopyTransformed(verts, vertPos, batch, batches.batchPos, count, world);

                    vertPos += count;
                    vertsLeft -= count;

                    if (batches.batchCurrent == batches.batches.Count - 1)
                    {
                        batches.batches.Add(new VertexPositionColorTexture[maxVertsPerBatch]);
                    }
                    batches.batchCurrent++;
                    batch = batches.batches[batches.batchCurrent];
                    batches.batchPos = 0;

                    // Started a new batch. Ensure strip is continuous
                    // copy last two verts across
                    if (vertsLeft > 0)
                    {
                        CopyTransformed(verts, vertPos - 2, batch, 0, 2, world);
                        batches.batchPos = 2;
                    }
                }
            }

            // Copy remaining verts to the current batch
            CopyTransformed(verts, vertPos, batch, batches.batchPos, vertsLeft, world);
            
            batches.batchPos += vertsLeft;

            hasTriangles = true;
        }

        Matrix tmpTransform;
        public void CopyTransformed(VertexPositionColorTexture[] source, int sourceIndex,
                                    VertexPositionColorTexture[] dest, int destIndex, int count, Matrix? transform)
        {
            int s = sourceIndex;
            int dEnd = count + destIndex;
            if (transform.HasValue)
            {
                tmpTransform = transform.Value;

                for (int d = destIndex; d < dEnd; d++)
                {
                    Vector3.Transform(ref source[s].Position, ref tmpTransform, out dest[d].Position);
                    dest[d].Color = source[s].Color;
                    dest[d].TextureCoordinate = source[s].TextureCoordinate;
                    s++;
                }
            }
            else
            {
                Array.Copy(source, sourceIndex, dest, destIndex, count);
            }
        }

        public void Reset()
        {
            foreach (BatchList bl in textureBatches.Values)
            {
                bl.batchCurrent = -1;
                bl.batchPos = 0;
            }
            foreach (BatchList bl in textureStripBatches.Values)
            {
                bl.batchCurrent = -1;
                bl.batchPos = 0;
            }

            hasTriangles = false;
        }


#if DEBUG
        public int maxTriCount;
        public int avgTriCount;
        public int texturesCount;
#endif

        public void Render(ref Matrix world, ref Matrix view, ref Matrix proj)
        {
            if (!hasTriangles)
                return;

            fxMatrices.World = world;
            fxMatrices.View = view;
            fxMatrices.Projection = proj;

            for (int iPass = 0; iPass < Effect.CurrentTechnique.Passes.Count; iPass++)
            {
                EffectPass pass = Effect.CurrentTechnique.Passes[iPass];
                
#if DEBUG
                int triCount = 0;
                int triCountTotal = 0;
#endif
                foreach (BatchList batches in batchesToRender)
                {
                    // skip empty texture batches
                    if (batches.batchCurrent < 0)
                        continue;

#if DEBUG
                    triCount = 0;
#endif
                    
                    // render all batches with this texture
                    fxParamTexture.SetValue(batches.texture);
                    pass.Apply();
                    
                    // draw batch primitives
                    for (int i = 0; i < batches.batchCurrent; i++)
                    {
                        try
                        {
                            device.DrawUserPrimitives<VertexPositionColorTexture>(PrimitiveType.TriangleList, batches.batches[i], 0, maxPrimitivesPerBatch);
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine(ex.ToString());
                        }
#if DEBUG
                        triCount += maxPrimitivesPerBatch;
#endif
                    }

                    try
                    {
                        device.DrawUserPrimitives<VertexPositionColorTexture>(PrimitiveType.TriangleList, batches.batches[batches.batchCurrent], 0, batches.batchPos / 3);
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine(ex.ToString());
                    }
#if DEBUG
                    triCount += batches.batchPos / 3;
                    triCountTotal += triCount;

                    maxTriCount = Math.Max(maxTriCount, triCount);
#endif
                }

#if DEBUG
                if (textureBatches.Count > 0)
                {
                    avgTriCount = triCountTotal / textureBatches.Count;
                    maxTriCount = Math.Max(maxTriCount, triCount);
                    texturesCount = textureBatches.Count;
                }
#endif
            }
        
        }
    }
}
