using UnityEngine;


public static class KeyPressCheck
{
    public static readonly KeyCheck Mouse0Key = new KeyCheck(KeyCode.Mouse0), 
                                    Mouse1Key = new KeyCheck(KeyCode.Mouse1);

    public class KeyCheck
    {
        public IKeyOwner owner;
        public readonly KeyCode keyCode;

        public KeyCheck(KeyCode keyCode)
        {
            this.keyCode = keyCode;
        }

        public bool Down(IKeyOwner owner)
        {
            if (Input.GetKeyDown(keyCode) && this.owner == null)
            {
                this.owner = owner;

                return true;
            }

            return false;
        }
        
        public bool Hold(IKeyOwner owner)
        {
            if (Input.GetKey(keyCode) && (this.owner == null || this.owner == owner))
            {
                this.owner = owner;

                return true;
            }

            return false;
        }
        
        
        public bool Up(IKeyOwner owner)
        {
            if (Input.GetKeyUp(keyCode) && this.owner == owner)
            {
                this.owner = null;

                return true;
            }

            return false;
        }
    }
    
    public interface IKeyOwner {}
}
