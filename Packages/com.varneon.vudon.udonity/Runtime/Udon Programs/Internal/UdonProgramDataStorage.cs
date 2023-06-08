using System;
using UdonSharp;
using UnityEngine;
using VRC.Udon;
using Varneon.VUdon.ArrayExtensions;
using Object = UnityEngine.Object;

namespace Varneon.VUdon.Udonity
{
    [AddComponentMenu("")]
    [DisallowMultipleComponent]
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class UdonProgramDataStorage : UdonSharpBehaviour
    {
        [SerializeField]
        internal string[] programLookupTable;

        [SerializeField]
        internal string[][] programSymbols;

        [SerializeField]
        internal Type[][] programSymbolTypes;

        [SerializeField]
        internal string[][] programEntryPoints;

        [SerializeField]
        internal Object[] programScripts;

        [SerializeField]
        internal Texture[] programScriptIcons;

        private const string
            TYPE_ID_SYMBOL = "__refl_typeid",
            TYPE_NAME_SYMBOL = "__refl_typename";

        public bool TryGetProgramIndex(UdonBehaviour udonBehaviour, out int index, out long id, out string name)
        {
            object typeid = udonBehaviour.GetProgramVariable(TYPE_ID_SYMBOL);

            object typename = udonBehaviour.GetProgramVariable(TYPE_NAME_SYMBOL);

            if (typeid == null || typename == null) { index = -1; id = 0; name = string.Empty; return false; }

            id = (long)typeid;

            name = (string)typename;

            string programInfo = $"{name} {id}";

            index = programLookupTable.IndexOf(programInfo);

            if(index < 0) { id = 0; name = string.Empty; return false; }

            return index >= 0;
        }

        public string[] GetProgramSymbols(int programIndex)
        {
            return programSymbols[programIndex];
        }

        public Type[] GetProgramSymbolTypes(int programIndex)
        {
            return programSymbolTypes[programIndex];
        }

        public string[] GetProgramEntryPoints(int programIndex)
        {
            return programEntryPoints[programIndex];
        }
    }
}
