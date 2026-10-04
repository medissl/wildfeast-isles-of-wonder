using System;
using System.IO;
namespace Wildfeast
{
    public sealed class JourneySlots
    {
        public const int Count=5;
        public readonly string DirectoryPath;
        public JourneySlots(string directory){DirectoryPath=directory;}
        public string PathFor(int slot){if(slot<0||slot>=Count)throw new ArgumentOutOfRangeException(nameof(slot));return Path.Combine(DirectoryPath,"journey-"+(slot+1)+".json");}
        public bool Exists(int slot)=>File.Exists(PathFor(slot));
        public int Empty(){for(int i=0;i<Count;i++)if(!Exists(i))return i;return -1;}
        public Progress Preview(int slot,Content content)
        {if(!Exists(slot))return null;try{return SaveStore.Parse(File.ReadAllText(PathFor(slot)),content);}catch{return null;}}
        public SaveStore Create(int slot,Progress state)
        {if(Exists(slot))throw new IOException("This journey already exists. Choose an empty slot.");var store=new SaveStore(PathFor(slot));store.Write(state);return store;}
    }
}
