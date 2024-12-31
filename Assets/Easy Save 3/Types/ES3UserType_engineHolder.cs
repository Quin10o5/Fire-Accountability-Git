using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("full", "currentCommander", "sizeMultiplier", "holderPositions", "holders", "insideOutsideVis", "insideOutsideMaterials", "searchCompletion", "companyNum", "companyNumText", "littlePeople", "littlePeopleOnAt", "enabled")]
	public class ES3UserType_engineHolder : ES3ComponentType
	{
		public static ES3Type Instance = null;

		public ES3UserType_engineHolder() : base(typeof(engineHolder)){ Instance = this; priority = 1;}


		protected override void WriteComponent(object obj, ES3Writer writer)
		{
			var instance = (engineHolder)obj;
			
			writer.WriteProperty("full", instance.full, ES3Type_bool.Instance);
			writer.WritePropertyByRef("currentCommander", instance.currentCommander);
			writer.WriteProperty("sizeMultiplier", instance.sizeMultiplier, ES3Type_float.Instance);
			writer.WriteProperty("holderPositions", instance.holderPositions, ES3UserType_TransformArray.Instance);
			writer.WriteProperty("holders", instance.holders, ES3Type_GameObjectArray.Instance);
			writer.WriteProperty("insideOutsideVis", instance.insideOutsideVis, ES3Type_MeshRendererArray.Instance);
			writer.WriteProperty("insideOutsideMaterials", instance.insideOutsideMaterials, ES3Type_MaterialArray.Instance);
			writer.WriteProperty("searchCompletion", instance.searchCompletion, ES3Type_int.Instance);
			writer.WriteProperty("companyNum", instance.companyNum, ES3Type_int.Instance);
			writer.WritePropertyByRef("companyNumText", instance.companyNumText);
			writer.WriteProperty("littlePeople", instance.littlePeople, ES3Type_GameObjectArray.Instance);
			writer.WriteProperty("littlePeopleOnAt", instance.littlePeopleOnAt, ES3Type_intArray.Instance);
			writer.WriteProperty("enabled", instance.enabled, ES3Type_bool.Instance);
		}

		protected override void ReadComponent<T>(ES3Reader reader, object obj)
		{
			var instance = (engineHolder)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "full":
						instance.full = reader.Read<System.Boolean>(ES3Type_bool.Instance);
						break;
					case "currentCommander":
						instance.currentCommander = reader.Read<UnityEngine.GameObject>(ES3Type_GameObject.Instance);
						break;
					case "sizeMultiplier":
						instance.sizeMultiplier = reader.Read<System.Single>(ES3Type_float.Instance);
						break;
					case "holderPositions":
						instance.holderPositions = reader.Read<UnityEngine.Transform[]>(ES3UserType_TransformArray.Instance);
						break;
					case "holders":
						instance.holders = reader.Read<UnityEngine.GameObject[]>(ES3Type_GameObjectArray.Instance);
						break;
					case "insideOutsideVis":
						instance.insideOutsideVis = reader.Read<UnityEngine.MeshRenderer[]>(ES3Type_MeshRendererArray.Instance);
						break;
					case "insideOutsideMaterials":
						instance.insideOutsideMaterials = reader.Read<UnityEngine.Material[]>(ES3Type_MaterialArray.Instance);
						break;
					case "searchCompletion":
						instance.searchCompletion = reader.Read<System.Int32>(ES3Type_int.Instance);
						break;
					case "companyNum":
						instance.companyNum = reader.Read<System.Int32>(ES3Type_int.Instance);
						break;
					case "companyNumText":
						instance.companyNumText = reader.Read<TMPro.TMP_Text>();
						break;
					case "littlePeople":
						instance.littlePeople = reader.Read<UnityEngine.GameObject[]>(ES3Type_GameObjectArray.Instance);
						break;
					case "littlePeopleOnAt":
						instance.littlePeopleOnAt = reader.Read<System.Int32[]>(ES3Type_intArray.Instance);
						break;
					case "enabled":
						instance.enabled = reader.Read<System.Boolean>(ES3Type_bool.Instance);
						break;
					default:
						reader.Skip();
						break;
				}
			}
		}
	}


	public class ES3UserType_engineHolderArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3UserType_engineHolderArray() : base(typeof(engineHolder[]), ES3UserType_engineHolder.Instance)
		{
			Instance = this;
		}
	}
}