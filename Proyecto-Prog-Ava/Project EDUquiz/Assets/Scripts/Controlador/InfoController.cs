using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Clase que permite conectar la vista de 'Informacion' y los componentes que se encuentran en
/// el editor Unity.
/// </summary>
public class InfoController : MonoBehaviour {
	public Canvas ui_main;
	public Canvas ui_informacion;
	public TextMeshProUGUI txtVersion; //asigna la version actual en la que se encuentra la APP

	/// <summary>
	/// Este metodo es el primero en iniciarse y se ejecuta solo una vez, necesario para referenciar
	/// componentes o definir el valor inicial de una variable, en este caso utilizado para cargar
	/// la version actual de la app en una caja de texto.
	/// </summary>
	void Awake () {
		txtVersion.text = "Version: " + Application.version;
	}

	/// <summary>
	/// Este metodo es ejecutado mediante un boton, para abrir un navegador con la URL deseada que 
	/// en este caso es la pagina de inicio de EDUquiz.
	/// </summary>
	/// <remarks>
	/// Es necesario que este método sea publico para que pueda ser referenciado mediante el editor
	/// de Unity
	/// </remarks>
	public void btnIrPaginaWeb () {
		Application.OpenURL ("https://eduquiz.000webhostapp.com/cliente/index.html");
	}

	/// <summary>
	/// Este método es ejecutado mediante un boton, para volver a la pantalla 'principal' del juego.
	/// </summary>
	/// <remarks>
	/// Es necesario que este método sea publico para que pueda ser referenciado mediante el editor
	/// de Unity
	/// </remarks>
	public void btnRegresarMenu () {
		ui_informacion.enabled = false;
		ui_main.enabled = true;
	}
}