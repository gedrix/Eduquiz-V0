using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Clase que es utilizada para instanciar objetos en Unity con los atributos y metodos definidos
/// </summary>
public class ControlItemLogro : MonoBehaviour {
	//[HideInInspector] Oculta el parametro en el editor Unity para evitar modificarlo erroneamente
	[HideInInspector]
	public int id; //almacena el id del logro, para saber si este fue desbloqueado.
	public TextMeshProUGUI txtTitulo; //muestra titulo del logro.
	public TextMeshProUGUI txtDescripcion; //muestra la descripcion para alcanzar el logro.
	public GameObject bloqueo; //obtengo el objeto para poder activarlo/desactivarlo a mi favor.
	public string estado = "Bloqueado";//permite conocer el estado del logro actual.
	ChangeImage[] img = new ChangeImage[2];
	/// <summary>
	/// Este metodo es el primero en iniciarse y se ejecuta solo una vez, necesario para referenciar
	/// componentes o definir el valor inicial de una variable, en este caso utilizado para hacer
	/// referencia a dos objetos hijos que tengan las clases <ChangeImage> de los componente padre.
	/// </summary>
	void Awake () {
		img = this.GetComponentsInChildren<ChangeImage> ();
	}

	/// <summary>
	/// Este método esta encargado de settear la informacion al momento de que un objeto sea 
	/// instanciado en Unity.
	/// </summary>
	/// <param name="idLogro">Para almacenar el identificador del logro</param>
	/// <param name="titulo">Para mostrar el titulo del logro</param>
	/// <param name="descripcion">Para mostrar la descripcion del logro</param>
	/// <param name="imagen">Nombre de la imagen del logro para que sea cargada</param>
	public void editarInfoItem (int idLogro, string titulo, string descripcion, string imagen = "eduquiz") {
		id = idLogro;
		txtTitulo.text = titulo;
		txtDescripcion.text = descripcion;
		// buscar detro de las clases el objeto que contenga el tag 'Logro/Imagen' para poder 
		// obtener y/o editar su imagen.
		for (int i = 0; i < img.Length; i++) {
			if (img[i].gameObject.CompareTag ("Logro/Imagen")) {
				if (imagen != "eduquiz") {
					img[i].SetImage (Resources.Load<Sprite> ("Images/Logros/" + imagen));
				} else
					img[i].SetImage (Resources.Load<Sprite> ("Images/Logo/" + imagen));
				break;
			}
		}
	}

	/// <summary>
	/// Este método permite mostrar el logro seleccionado como 'Desbloqueado' modificando su
	/// apariencia como su icono y su forma 'semi-oculta'.
	/// </summary>
	public void desbloquearLogro () {
		// buscar detro de las clases el objeto que contenga el tag 'Logro/Bloqueo' para poder 
		// obtener y/o editar su imagen y tambien desactiva el 'bloqueo' del logro.
		for (int i = 0; i < img.Length; i++) {
			if (img[i].gameObject.CompareTag ("Logro/Bloqueo")) {
				img[i].SetImage (Resources.Load<Sprite> ("Images/Game/iconDesbloqueado"));
			}
		}
		bloqueo.SetActive (false);
		estado = "Desbloqueado";
	}
}