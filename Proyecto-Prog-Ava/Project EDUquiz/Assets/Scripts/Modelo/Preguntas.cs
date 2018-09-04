using System;
using UnityEngine;

/// <summary>
/// Esta clase se encarga de recolectar los id's de las preguntas recibidas aleatoriamente
/// para consultar cada una de ellas.
/// </summary>
/// <remarks>
/// NOTA: los 'modelos' necesariamente deben incluir al inicio [Serializable], lo cual serializa
/// cada uno de los parametros y permite que Unity identifique a esta clase como un objeto 'modelo'. 
/// </remarks>
[Serializable]
public class Preguntas {
	public int[] ids; //es necesario conocer el id de la pregunta para evitar la redundancia de la misma en una partida.
	public string[] preguntas; //nombre o titulo de la pregunta
	public string[] dificultad; //dificultad de cada una de las preguntas
}