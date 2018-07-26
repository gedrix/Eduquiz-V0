using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ControlItemClasificacion : MonoBehaviour 
{
	public TextMeshProUGUI txtNombreUsu;//nombre del usuario
	public TextMeshProUGUI txtPuntaje;
	public TextMeshProUGUI txtPosicion;

	public void editarInfoItem(string nombre, int puntaje, int pos){
		txtNombreUsu.text = nombre;
		txtPuntaje.text = puntaje+"";
		txtPosicion.text = pos+"";
	}
}
