using System;
using UnityEngine;
/*
	Este modelo es diferente al de <Persona>, porque en este obtendre todas las personas
	de la consulta que se genera la lista de la clasificacion con campos limitados a solo
	la informacion basica...
 */
[Serializable]
public class Personas {
	public string[] nombres;
	public string[] imagenes;
	public int[] niveles;
	public int[] puntajes;
}