using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using UdonSharp;
using UnityEditor;
using UnityEngine;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;

namespace Varneon.VUdon.UdonProgramDataStorage
{
    public class UdonProgramDataStorageGenerator
    {
        private const string
            TYPE_ID_SYMBOL = "__refl_typeid",
            TYPE_NAME_SYMBOL = "__refl_typename";

        public static void GenerateProgramDataStorage()
        {
            IEnumerable<UdonBehaviour> behaviours = Resources.FindObjectsOfTypeAll<UdonBehaviour>().Where(u => u.gameObject.scene.IsValid());

            HashSet<AbstractUdonProgramSource> programSources = new HashSet<AbstractUdonProgramSource>(behaviours.Select(b => b.programSource));

            List<string> programInfos = new List<string>();

            List<string[]> programSymbols = new List<string[]>();

            List<Type[]> programSymbolTypes = new List<Type[]>();

            List<string[]> programEntryPoints = new List<string[]>();

            List<MonoScript> programScripts = new List<MonoScript>();

            List<Texture> programScriptIcons = new List<Texture>();

            foreach (AbstractUdonProgramSource programSource in programSources)
            {
                if (programSource == null) { continue; }

                IUdonProgram program = programSource.SerializedProgramAsset.RetrieveProgram();

                IUdonSymbolTable symbolTable = program.SymbolTable;

                ImmutableArray<string> symbols = symbolTable.GetSymbols();

                IUdonHeap heap = program.Heap;

                string header = programSource.name;

                if(symbols.Contains(TYPE_ID_SYMBOL) && symbols.Contains(TYPE_NAME_SYMBOL))
                {
                    long id = heap.GetHeapVariable<long>(symbolTable.GetAddressFromSymbol(TYPE_ID_SYMBOL));
                    string name = heap.GetHeapVariable<string>(symbolTable.GetAddressFromSymbol(TYPE_NAME_SYMBOL));

                    header = $"{name} {id}";
                }
                else
                {
                    // Program is not UdonSharpBehaviour

                    continue;
                }

                programInfos.Add(header);

                IEnumerable<string> filteredSymbols = symbols.Sort();//.Where(s => !s.StartsWith("__"));

                programSymbols.Add(filteredSymbols.ToArray());

                programSymbolTypes.Add(filteredSymbols.Select(s => symbolTable.GetSymbolType(s)).ToArray());

                IUdonSymbolTable entryPointTable = program.EntryPoints;

                IEnumerable<string> filteredEntryPoints = entryPointTable.GetSymbols().Where(s => !s.StartsWith("_onVarChange_")).ToImmutableSortedSet();

                programEntryPoints.Add(filteredEntryPoints.ToArray());

                MonoScript programScript = ((UdonSharpProgramAsset)programSource).sourceCsScript;

                programScripts.Add(programScript);

                Texture programScriptIcon = AssetDatabase.GetCachedIcon(AssetDatabase.GetAssetPath(programScript));

                programScriptIcons.Add(programScriptIcon.hideFlags.HasFlag(HideFlags.DontSave) ? null : programScriptIcon);
            }

            Udonity.UdonProgramDataStorage dataStorage = Resources.FindObjectsOfTypeAll<Udonity.UdonProgramDataStorage>().Where(s => s.gameObject.scene.IsValid()).FirstOrDefault();

            if (dataStorage)
            {
                Undo.RecordObject(dataStorage, "Generate UdonProgramDataStorage");

                dataStorage.programLookupTable = programInfos.ToArray();

                dataStorage.programSymbols = programSymbols.ToArray();

                dataStorage.programSymbolTypes = programSymbolTypes.ToArray();

                dataStorage.programEntryPoints = programEntryPoints.ToArray();

                dataStorage.programScripts = programScripts.ToArray();

                dataStorage.programScriptIcons = programScriptIcons.ToArray();
            }
        }
    }
}
