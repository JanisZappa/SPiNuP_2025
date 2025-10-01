using System.Collections.Generic;
using UnityEngine;

namespace Test
{
    public class LetterSet : MonoBehaviour
    {
        public string abc;
        public GameObject font;
        public string whattowrite;
        public Vector2Int dimensions;
        public Vector2 gaps;
        public GameObject dummy;

        private Dictionary<char, int> map;
        private Mesh[] alphabet;
        private Vector3[] offsets;
        private float[] widths;


        private void Start()
        {
            int count = font.transform.childCount;
            alphabet = new Mesh[count];
            offsets = new Vector3[count];
            widths = new float[count];
            for (int i = 0; i < count; i++)
            {
                Mesh m = font.transform.GetChild(i).GetComponent<MeshFilter>().sharedMesh;
                Bounds b = m.bounds;
                Vector3 offset = b.center;
                Vector3[] verts = m.vertices;
                for (int j = 0; j < verts.Length; j++)
                    verts[j] -= offset;

                offsets[i] = offset;
                widths[i] = b.size.x;

                Mesh newMesh = new Mesh { vertices = verts, triangles = m.triangles, colors = m.colors };
                newMesh.RecalculateBounds();

                alphabet[i] = newMesh;
            }

            map = new Dictionary<char, int>();
            for (int i = 0; i < abc.Length; i++)
                map.Add(abc[i], i);

            count = Mathf.Min(whattowrite.Length, dimensions.x * dimensions.y);
            Vector2 size = new Vector2((dimensions.x - 1) * gaps.x, (dimensions.y - 1) * gaps.y);
            Vector3 start = new Vector2(0, size.y * .5f);
            int currentY = 0;
            float xPos = 0;
            List<ForceRotationTest> rowTransforms = new List<ForceRotationTest>();
            Vector3 leftShift;
            for (int i = 0; i < count; i++)
            {
                char c = whattowrite[i];
                if (c == ' ')
                {
                    xPos += gaps.x * 2.4f;
                    continue;
                }

                int y = i / dimensions.x;

                if (y != currentY)
                {
                    leftShift = Vector3.right * (xPos - gaps.x) * -.5f;
                    for (int j = 0; j < rowTransforms.Count; j++)
                    {
                        rowTransforms[j].transform.localPosition += leftShift;
                        rowTransforms[j].Init();
                    }

                    rowTransforms.Clear();
                    currentY = y;
                    xPos = 0;
                }



                c = c.ToString().ToUpper()[0];
                if (!map.ContainsKey(c))
                    c = '#';

                int index = map[c];
                float width = widths[index];

                xPos += width * .5f;

                Vector3 pos = start + new Vector3(xPos, -y * gaps.y) + offsets[index];

                GameObject gO = Instantiate(dummy, transform);
                gO.transform.localPosition = pos;
                gO.GetComponent<MeshFilter>().mesh = alphabet[index];
                rowTransforms.Add(gO.GetComponent<ForceRotationTest>());

                xPos += width * .5f + gaps.x;
            }

            leftShift = Vector3.right * (xPos - gaps.x) * -.5f;
            for (int j = 0; j < rowTransforms.Count; j++)
            {
                rowTransforms[j].transform.localPosition += leftShift;
                rowTransforms[j].Init();
            }
        }
    }
}