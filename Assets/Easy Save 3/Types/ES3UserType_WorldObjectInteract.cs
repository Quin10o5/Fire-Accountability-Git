using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("isDragging", "company", "eH", "offset", "dragPlane", "enginePrefab", "lastSelected", "lastSelectedMaterial", "lookPoint", "enginesSO", "SOindex", "index", "selectedMat", "baseMat", "visRenderer", "m_CancellationTokenSource", "enabled")]
	public class ES3UserType_WorldObjectInteract : ES3ComponentType
	{
		public static ES3Type Instance = null;

		public ES3UserType_WorldObjectInteract() : base(typeof(WorldObjectInteract)){ Instance = this; priority = 1;}


		protected override void WriteComponent(object obj, ES3Writer writer)
		{
			var instance = (WorldObjectInteract)obj;
			
			writer.WritePrivateField("isDragging", instance);
			writer.WriteProperty("company", instance.company, ES3Type_int.Instance);
			writer.WritePropertyByRef("eH", instance.eH);
			writer.WritePrivateField("offset", instance);
			writer.WritePrivateField("dragPlane", instance);
			writer.WritePrivateFieldByRef("enginePrefab", instance);
			writer.WritePrivateFieldByRef("lastSelected", instance);
			writer.WritePrivateFieldByRef("lastSelectedMaterial", instance);
			writer.WritePrivateField("lookPoint", instance);
			writer.WritePropertyByRef("enginesSO", instance.enginesSO);
			writer.WriteProperty("SOindex", instance.SOindex, ES3Type_int.Instance);
			writer.WriteProperty("index", instance.index, ES3Type_int.Instance);
			writer.WritePropertyByRef("selectedMat", instance.selectedMat);
			writer.WritePropertyByRef("baseMat", instance.baseMat);
			writer.WritePropertyByRef("visRenderer", instance.visRenderer);
			writer.WritePrivateField("m_CancellationTokenSource", instance);
			writer.WriteProperty("enabled", instance.enabled, ES3Type_bool.Instance);
		}

		protected override void ReadComponent<T>(ES3Reader reader, object obj)
		{
			var instance = (WorldObjectInteract)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "isDragging":
					instance = (WorldObjectInteract)reader.SetPrivateField("isDragging", reader.Read<System.Boolean>(), instance);
					break;
					case "company":
						instance.company = reader.Read<System.Int32>(ES3Type_int.Instance);
						break;
					case "eH":
						instance.eH = reader.Read<engineHolder>();
						break;
					case "offset":
					instance = (WorldObjectInteract)reader.SetPrivateField("offset", reader.Read<UnityEngine.Vector3>(), instance);
					break;
					case "dragPlane":
					instance = (WorldObjectInteract)reader.SetPrivateField("dragPlane", reader.Read<UnityEngine.Plane>(), instance);
					break;
					case "enginePrefab":
					instance = (WorldObjectInteract)reader.SetPrivateField("enginePrefab", reader.Read<UnityEngine.GameObject>(), instance);
					break;
					case "lastSelected":
					instance = (WorldObjectInteract)reader.SetPrivateField("lastSelected", reader.Read<UnityEngine.MeshRenderer>(), instance);
					break;
					case "lastSelectedMaterial":
					instance = (WorldObjectInteract)reader.SetPrivateField("lastSelectedMaterial", reader.Read<UnityEngine.Material>(), instance);
					break;
					case "lookPoint":
					instance = (WorldObjectInteract)reader.SetPrivateField("lookPoint", reader.Read<UnityEngine.Vector3>(), instance);
					break;
					case "enginesSO":
						instance.enginesSO = reader.Read<enginesSO>();
						break;
					case "SOindex":
						instance.SOindex = reader.Read<System.Int32>(ES3Type_int.Instance);
						break;
					case "index":
						instance.index = reader.Read<System.Int32>(ES3Type_int.Instance);
						break;
					case "selectedMat":
						instance.selectedMat = reader.Read<UnityEngine.Material>(ES3Type_Material.Instance);
						break;
					case "baseMat":
						instance.baseMat = reader.Read<UnityEngine.Material>(ES3Type_Material.Instance);
						break;
					case "visRenderer":
						instance.visRenderer = reader.Read<UnityEngine.MeshRenderer>(ES3Type_MeshRenderer.Instance);
						break;
					case "m_CancellationTokenSource":
					instance = (WorldObjectInteract)reader.SetPrivateField("m_CancellationTokenSource", reader.Read<System.Threading.CancellationTokenSource>(), instance);
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


	public class ES3UserType_WorldObjectInteractArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3UserType_WorldObjectInteractArray() : base(typeof(WorldObjectInteract[]), ES3UserType_WorldObjectInteract.Instance)
		{
			Instance = this;
		}
	}
}