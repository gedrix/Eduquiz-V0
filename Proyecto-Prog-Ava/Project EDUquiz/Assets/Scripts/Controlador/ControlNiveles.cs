﻿using System.Collections;
using UnityEngine;

public class ControlNiveles {
	private int expMax = 80, expAnt = 0, expIncremento = 10; //exp limite, exp anterior, lo que se incrementara al sig nivel
	private int nivel = 0; //0 por defecto
	private int porcentaje = 0; //porcentaje de la experiencia actual para llegar al sig nivel...
	private int pDificil = 0, pNormal = 0, pFacil = 0;

	//devuelve [Nivel, Porcentaje, EXP maxima]
	public int[] obtenerNivelUsuario (int experiencia) {
		//reinicia los valores por defecto...
		expMax = 80;
		expAnt = 0;
		expIncremento = expMax + 10; //inicial de cuantos puntos se incrementaran por cada subida de nivel
		nivel = 1;
		//calcula el nivel
		for (int i = 0; i < 25; i++) // 25 niveles...
		{
			if (experiencia >= expMax) {
				expAnt = expMax; // la experiencia maxima pasa a ser exp anterior, porque sera incrementada...
				expMax = expIncremento + expMax; //incremento el tope de la experiencia para el siguiente nivel...
				nivel++;
				expIncremento += 10; //se incrementa 10 puntos extra para la experiencia max del sig nivel
			} else {
				break;
			}
		}

		//calcular porcentaje de la experiencia actual
		experiencia = experiencia - expAnt;
		expMax = expMax - expAnt;
		porcentaje = (experiencia * 100) / expMax;
		int[] infoNivel = new int[3] { nivel, porcentaje, expMax };
		return infoNivel;
	}
	//devuelve [puntos experiencia, puntos(clasificacion)]
	public int[] obtenerPuntosPorDificultad (int nivel, string dificultad) {
		int[] infoPuntaje = new int[2] { 10, 1 }; //por defecto [10 puntos de experiencia, 1 punto (clasificacion)]

		switch (dificultad) {
			case "Facil":
				if (nivel <= 5) //facil
					infoPuntaje[0] = 10;
				if (nivel > 5 && nivel <= 10) //normal
					infoPuntaje[0] = 8;
				if (nivel > 10 && nivel <= 15) //dificil
					infoPuntaje[0] = 6;
				if (nivel > 15 && nivel <= 20) //muy dificil
					infoPuntaje[0] = 4;
				if (nivel > 20) //maestro
					infoPuntaje[0] = 2;
				pFacil++;
				if (pFacil >= 3) {
					pFacil -= 3;
					infoPuntaje[1] = 1; //punto (clasificacion)
				} else
					infoPuntaje[1] = 0; //punto (clasificacion)
				return infoPuntaje;
				break;
			case "Normal":
				if (nivel <= 5) //facil
					infoPuntaje[0] = 10;
				if (nivel > 5 && nivel <= 10) //normal
					infoPuntaje[0] = 10;
				if (nivel > 10 && nivel <= 15) //dificil
					infoPuntaje[0] = 8;
				if (nivel > 15 && nivel <= 20) //muy dificil
					infoPuntaje[0] = 6;
				if (nivel > 20) //maestro
					infoPuntaje[0] = 4;
				pNormal++;
				if (pNormal >= 2) {
					pNormal -= 2;
					infoPuntaje[1] = 1; //punto (clasificacion)
				} else
					infoPuntaje[1] = 0; //punto (clasificacion)
				return infoPuntaje;
				break;
			case "Dificil":
				if (nivel <= 5) //facil
					infoPuntaje[0] = 10;
				if (nivel > 5 && nivel <= 10) //normal
					infoPuntaje[0] = 10;
				if (nivel > 10 && nivel <= 15) //dificil
					infoPuntaje[0] = 10;
				if (nivel > 15 && nivel <= 20) //muy dificil
					infoPuntaje[0] = 8;
				if (nivel > 20) //maestro
					infoPuntaje[0] = 6;
				pDificil++;
				if (pDificil >= 2) {
					pDificil -= 2;
					infoPuntaje[1] = 1; //punto (clasificacion)
				} else
					infoPuntaje[1] = 0; //punto (clasificacion)
				return infoPuntaje;
				break;
			case "Muy Dificil":
				if (nivel <= 5) //facil
					infoPuntaje[0] = 10;
				if (nivel > 5 && nivel <= 10) //normal
					infoPuntaje[0] = 10;
				if (nivel > 10 && nivel <= 15) //dificil
					infoPuntaje[0] = 10;
				if (nivel > 15 && nivel <= 20) //muy dificil
					infoPuntaje[0] = 10;
				if (nivel > 20) //maestro
					infoPuntaje[0] = 8;
				infoPuntaje[1] = 1; //punto (clasificacion)
				return infoPuntaje;
				break;
			case "Maestro":
				infoPuntaje[0] = 10;
				infoPuntaje[1] = 1; //punto (clasificacion)
				return infoPuntaje;
		}

		return infoPuntaje;
	}
}