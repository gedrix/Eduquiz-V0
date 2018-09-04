using System;
using UnityEngine;

/// <summary>
/// Este modelo es diferente al de <Persona>, porque en este obtendre todas las personas
/// de la consulta que se genera la lista de la clasificacion con campos limitados a solo
/// la informacion basica, para listar la clasificacion de los jugadores.
/// </summary>
/// <remarks>
/// NOTA: los 'modelos' necesariamente deben incluir al inicio [Serializable], lo cual serializa
/// cada uno de los parametros y permite que Unity identifique a esta clase como un objeto 'modelo'. 
/// </remarks>
[Serializable]
public class Personas {
	public string[] nombres; //lista de nombres de los jugadores
	public string[] correos; //lista de correos de los jugadores
	public string[] imagenes; //lista de nombres de las imagenes
	public int[] puntajes; //lista de los puntajes en la clasificacion
}