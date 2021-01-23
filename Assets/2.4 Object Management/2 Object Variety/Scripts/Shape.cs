/*
 * Author: Huai
 * Create: 2021/1/23 23:04:53
 *
 * Description:
 */

using System;
using UnityEngine;
using ObjecManagement.PersistingObjects;

namespace ObjecManagement.ObjectVariety
{
    public class Shape : PersistableObject
    {
        // 默认渲染管线材质颜色属性名是_Color，在URP中材质的颜色属性名变成了_BaseColor
        private static readonly int colorPropertyId = Shader.PropertyToID("_Color");

        private static MaterialPropertyBlock sharedPropertyBlock;

        public int ShapeId
        {
            get => shapeId;
            set
            {
                if (shapeId == -1)
                {
                    shapeId = value;
                }
                else
                {
                    Debug.LogError("Not allowed to change shapeId");
                }
            }
        }

        public int MaterialId { get; private set; }

        private int shapeId = -1;
        private Color color;
        private MeshRenderer meshRenderer;

        private void Awake()
        {
            meshRenderer = GetComponent<MeshRenderer>();
        }

        public void SetMaterial(Material material, int materialId)
        {
            MaterialId = materialId;
            meshRenderer.material = material;
        }

        public void SetColor(Color color)
        {
            this.color = color;

            // 设置材质的颜色会导致每次都重新生成一个material
            // 使用MaterialPropertyBlock可以避免这种情况
            // meshRenderer.material.color = color;
            if (sharedPropertyBlock == null)
            {
                sharedPropertyBlock = new MaterialPropertyBlock();
            }
            sharedPropertyBlock.SetColor(colorPropertyId, color);
            meshRenderer.SetPropertyBlock(sharedPropertyBlock);
        }

        public override void Save(DataWriter writer)
        {
            base.Save(writer);
            writer.Write(color);
        }

        public override void Load(DataReader reader)
        {
            base.Load(reader);
            SetColor(reader.Version > 0 ? reader.ReadColor() : Color.white);
        }
    }
}
