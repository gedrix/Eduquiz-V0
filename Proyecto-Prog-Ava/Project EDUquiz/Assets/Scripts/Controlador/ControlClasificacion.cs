using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlClasificacion : MonoBehaviour 
{
	public GameObject itemClasificacion; //prefab del objeto -> itemClasificacion para generar clones del mismo
	public Transform trnsItemClas; //pivote que me sirve de referencia para la instancia de objetos
	Personas personas;
	//referencia a otros scripts con Monobehaviour
	ControlItemClasificacion controlItem;
	List<GameObject> listaItems = new List<GameObject>();

	public void obtenerListaClasificacion (string token) {
		string json = "{}";
		string url = "https://eduquiza.azurewebsites.net/public/index.php/usuario/listaClasificacionJuego";

		Dictionary<string, string> headers = new Dictionary<string, string> ();
		headers.Add ("Content-Type", "application/json; charset=utf-8");
		headers.Add ("token", token);

		byte[] pData = System.Text.Encoding.UTF8.GetBytes (json.ToCharArray ());

		WWW www = new WWW (url, pData, headers);
		StartCoroutine (requestLista (www));
	}
	IEnumerator requestLista (WWW www) {
		yield return www;

		if (string.IsNullOrEmpty (www.error)) {
			personas = JsonUtility.FromJson<Personas> (www.text);
			limpiarListaItems();
			for (int i = 0; i < personas.nombres.Length; i++) {
				GameObject obj = (GameObject) Instantiate (itemClasificacion);
				listaItems.Add(obj);
				obj.transform.parent = trnsItemClas.parent;
				obj.transform.localScale = new Vector3(1f, 1f, 1f);
				//obtengo el componente para editar la info de cada item
				controlItem = obj.GetComponent<ControlItemClasificacion>();
				controlItem.editarInfoItem(personas.nombres[i], personas.puntajes[i], i+1);
			}
		} else
			print ("Error al listar clasificacion. "+ www.error);
		//txtMensajeRegistrar.text = "<color=red>Error:\n</color>" + www.error; //error
		//aqui deberia parar la 'carga' y habilitar el boton...
	}
	private void limpiarListaItems(){
		foreach (GameObject item in listaItems)
		{
			Destroy(item);
		}
		listaItems.Clear();
	}
}