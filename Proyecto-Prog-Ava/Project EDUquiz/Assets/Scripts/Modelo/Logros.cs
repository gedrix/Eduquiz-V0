using System;
using UnityEngine;

/// <summary>
/// Este modelo se encarga de almacenar cada uno de los logros que son obtenidos de la base de datos.
/// </summary>
/// <remarks>
/// NOTA: los 'modelos' necesariamente deben incluir al inicio [Serializable], lo cual serializa
/// cada uno de los parametros y permite que Unity identifique a esta clase como un objeto 'modelo'. 
/// </remarks>
[Serializable]
public class Logros {
	public int[] ids; //almacena los ids de la lista de logros recibida para saber cual esta 'desbloqueado'
	public string[] nombres; //almacena los nombre o titulos de cada logro.
	public string[] descripciones; //almacena cada una de las descripciones de los logros.
	public string[] imagenes; //almacena el nombre de la imagen asignada para cada logro.
}