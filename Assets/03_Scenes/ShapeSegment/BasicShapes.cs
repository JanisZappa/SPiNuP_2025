using UnityEngine;


namespace ShapeStuff
{
    public partial class Shape
    {
        private void Create_WigglyLine()
        {
            segmentCount = 10;
            
            for (int i = 0; i < segmentCount; i++)
                (i == 0 ? segments[0].Randomize() : segments[i].Connect(segments[i - 1])).SetLengthBend(Random.Range( 1, 3), Random.Range(-.8f, .8f));
            
            
            FinishSetup();
        }
        
        
        private void Create_Circle()
        {
            segmentCount = 1;
            segments[0].Randomize().SetLengthBend(Random.Range(4, 20f), Sign.Random);
            
            FinishSetup();
        }
        
        
        private void Create_Quad()
        {
            segmentCount = 8;
            
            float sideLength   = Random.Range(.5f, 4);
            float cornerLength = Random.Range(.5f, 4);

            float extra = Random.Range(-.2f, .2f);

            float dir = .25f * Sign.Random;
            segments[0].Randomize().         SetLengthBend(cornerLength, dir + extra);
            segments[1].Connect(segments[0]).SetLengthBend(sideLength, -extra);
            segments[2].Connect(segments[1]).SetLengthBend(cornerLength, dir + extra);
            segments[3].Connect(segments[2]).SetLengthBend(sideLength,        -extra);
            segments[4].Connect(segments[3]).SetLengthBend(cornerLength, dir + extra);
            segments[5].Connect(segments[4]).SetLengthBend(sideLength,        -extra);
            segments[6].Connect(segments[5]).SetLengthBend(cornerLength, dir + extra);
            segments[7].Connect(segments[6]).SetLengthBend(sideLength,        -extra);
            
            FinishSetup();
        }
        
        
        private void Create_Tri()
        {
            segmentCount = 6;
            
            float sideLength   = Random.Range(.5f, 4);
            float cornerLength = Random.Range(.5f, 4);
            
            float extra = Random.Range(-.2f, .2f);
            
            float dir = 1f / 3 * Sign.Random;
            segments[0].Randomize().         SetLengthBend(cornerLength, dir + extra);
            segments[1].Connect(segments[0]).SetLengthBend(sideLength,        -extra);
            segments[2].Connect(segments[1]).SetLengthBend(cornerLength, dir + extra);
            segments[3].Connect(segments[2]).SetLengthBend(sideLength,        -extra);
            segments[4].Connect(segments[3]).SetLengthBend(cornerLength, dir + extra);
            segments[5].Connect(segments[4]).SetLengthBend(sideLength,        -extra);
            
            FinishSetup();
        }
        
        
        private void Create_Pill()
        {
            segmentCount = 4;
            
            float sideLength   = Random.Range(.5f, 4);
            float cornerLength = Random.Range(.5f, 4);
            float extra        = Random.Range(-.3f, .3f);
            float dir          = .5f * Sign.Random;
            
            segments[0].Randomize().SetLengthBend(cornerLength,          dir + extra);
            segments[1].Connect(segments[0]).SetLengthBend(sideLength,        -extra);
            segments[2].Connect(segments[1]).SetLengthBend(cornerLength, dir + extra);
            segments[3].Connect(segments[2]).SetLengthBend(sideLength,        -extra);
            
            FinishSetup();
        }
        
        
        private void Create_WigglePill()
        {
            segmentCount = 6;
            
            float sideLength   = Random.Range(.5f, 4);
            float cornerLength = Random.Range(.5f, 4);

            float wiggle = Random.Range(-.2f, .2f);
            
            float dir = .5f * Sign.Random;
            segments[0].Randomize().SetLengthBend(cornerLength,         dir);
            segments[1].Connect(segments[0]).SetLengthBend(sideLength * .5f,  wiggle);
            segments[2].Connect(segments[1]).SetLengthBend(sideLength * .5f, -wiggle);
            segments[3].Connect(segments[2]).SetLengthBend(cornerLength,         dir);
            segments[4].Connect(segments[3]).SetLengthBend(sideLength * .5f,  wiggle);
            segments[5].Connect(segments[4]).SetLengthBend(sideLength * .5f, -wiggle);
           
            FinishSetup();
        }
        
        
        private void Create_BendPill()
        {
            segmentCount = 4;
            
           
            float cornerLength = Random.Range(.5f, 4);
            float cornerRadius = cornerLength / Mathf.PI;
            
            float sideLength   = Random.Range(cornerLength * 2, 12);
            float pillBend     = Random.Range(-.49f, .49f);
            float bendRadius   = sideLength * (1f / Mathf.Abs(pillBend)) * .5f/ 2 / Mathf.PI;
            
            float innerLength = (bendRadius - cornerRadius) * 2 * Mathf.PI * Mathf.Abs(pillBend);
            float outerLength = (bendRadius + cornerRadius) * 2 * Mathf.PI * Mathf.Abs(pillBend);

            bool leftBend = pillBend > 0;
            
            segments[0].Randomize().SetLengthBend(cornerLength, .5f);
            segments[1].Connect(segments[0]).SetLengthBend(leftBend? outerLength : innerLength, pillBend);
            segments[2].Connect(segments[1]).SetLengthBend(cornerLength, .5f);
            segments[3].Connect(segments[2]).SetLengthBend(leftBend? innerLength : outerLength, -pillBend);
            
            FinishSetup();
        }
        
        
        private void Create_Spiral()
        {
            segmentCount = 10;

            float startLength = Random.Range(1f, 4f);
            float shrinkage = 1f / segmentCount * Random.Range(.3f, .98f);

            float segmentBend = Random.Range(.05f, .4f) * Sign.Random;
            
            for (int i = 0; i < segmentCount; i++)
                (i == 0? segments[0].Randomize() : segments[i].Connect(segments[i - 1])).SetLengthBend(startLength * (1 - shrinkage * i), segmentBend);
           
            FinishSetup();
        }


        private void Create_Flower()
        {
            int segCount = Random.Range(2, 6);
            segmentCount = segCount * 2;

            float radius = Random.Range(1f, 1.5f);
            
            float segmentRad = 2 * Mathf.PI / segCount;
            float ratio      = Random.Range(.3f, .7f);
            
            float segmentA = segmentRad * ratio;
            float radA     = (.5f * Mathf.PI - segmentA * .5f) * 2;
            float radiusA  = radius * Mathf.Sin(segmentA * .5f) / Mathf.Sin(radA * .5f);
            float bendA    = 1 - radA / (2 * Mathf.PI);
            float lengthA  = 2 * Mathf.PI * radiusA * bendA;
            
            float segmentB   = segmentRad - segmentA;
            float radB     = (.5f * Mathf.PI - segmentB * .5f) * 2;
            float radiusB  = radius * Mathf.Sin(segmentB * .5f) / Mathf.Sin(radB * .5f);
            float bendB    = radB / (2 * Mathf.PI);
            float lengthB  = 2 * Mathf.PI * radiusB * bendB;

            int dir = Sign.Random;
            for (int i = 0; i < segCount; i++)
            {
                (i == 0? segments[0].Randomize() : segments[i * 2].Connect(segments[i * 2 - 1])).SetLengthBend(lengthA, -bendA * dir);
                segments[i * 2 + 1].Connect(segments[i * 2]).SetLengthBend(lengthB, bendB * dir);
            }
            
            FinishSetup();
        }
        
        
        private void Create_Splooch()
        {
            int segCount = Random.Range(2, 6);
            segmentCount = segCount * 2;

            float radius     = Random.Range(1f, 1.5f);
            float segmentRad = 2 * Mathf.PI / segCount;
           

            int dir = Sign.Random;
            for (int i = 0; i < segCount; i++)
            {
                float ratio      = Random.Range(.2f, .8f);
            
                float segmentA = segmentRad * ratio;
                float radA     = (.5f * Mathf.PI - segmentA * .5f) * 2;
                float radiusA  = radius * Mathf.Sin(segmentA * .5f) / Mathf.Sin(radA * .5f);
                float bendA    = 1 - radA / (2 * Mathf.PI);
                float lengthA  = 2 * Mathf.PI * radiusA * bendA;
            
                float segmentB = segmentRad - segmentA;
                float radB     = (.5f * Mathf.PI - segmentB * .5f) * 2;
                float radiusB  = radius * Mathf.Sin(segmentB * .5f) / Mathf.Sin(radB * .5f);
                float bendB    = radB / (2 * Mathf.PI);
                float lengthB  = 2 * Mathf.PI * radiusB * bendB;
                
                (i == 0? segments[0].Randomize() : segments[i * 2].Connect(segments[i * 2 - 1])).SetLengthBend(lengthA, -bendA * dir);
                segments[i * 2 + 1].Connect(segments[i * 2]).SetLengthBend(lengthB, bendB * dir);
            }
            
            FinishSetup();
        }


        public void CreateExtrude(Shape guideShape)
        {
            if (guideShape.loop)
            {
                segmentCount = 0;
                FinishSetup();
                return;
            }
            
            
            float width  = Mathf.Min(.14f, guideShape.PossibleThickness);
            segmentCount = guideShape.segmentCount * 2 + 2;
           
            
            float dir = 1;
            Segment before = null;
            int index = 0;

            for (int i = 0; i < 2; i++)
            {
                for (int e = 0; e < guideShape.segmentCount; e++)
                {
                    int   readIndex     = i == 0 ? e : guideShape.segmentCount - 1 - e; 
                    Segment readSegment = guideShape.segments[readIndex];
                    
                    
                    float radius        = readSegment.arc.SignedRadius;
                    float newRadius     = radius - width * dir;
                    float multi         = newRadius / radius;
                    float segmentLength = readSegment.length * multi;

                    if (before == null)
                        segments[0].SetPointDir(guideShape.GetPoint(0, -width), guideShape.segments[0].radA);
                    else
                        segments[index].Connect(before);
                    
                    segments[index].SetLengthBend(segmentLength, readSegment.Bend * dir);
                    before = segments[index];
                    index++;
                }


                float capLength = Mathf.PI * width;
                segments[index].Connect(before).SetLengthBend(capLength, -.5f);
                before = segments[index];
                index++;

                dir = -1;
            }

            FinishSetup();
        }

        
        public void CreateByNumber(int number)
        {
            switch (number)
            {
                case 0: Create_WigglyLine();    break;
                case 1: Create_Circle();        break;
                case 2: Create_Quad();          break;
                case 3: Create_Tri();           break;
                case 4: Create_Pill();          break;
                case 5: Create_WigglePill();    break;
                case 6: Create_BendPill();      break; 
                case 7: Create_Spiral();        break;
                case 8: Create_Flower();        break;
                case 9: Create_Splooch();       break;
            }
        }
    }
}
