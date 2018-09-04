using System.Collections;
using UnityEngine;

/// <summary>
/// Esta clase no es necesaria verla en el editor 'Unity' y por eso no hereda de <MonoBehaviour>
/// </summary>
public class ControlNiveles {
	private int expMax = 80, expAnt = 0, expIncremento = 10; //exp limite, exp anterior, lo que se incrementara al sig nivel
	private int nivel = 0; //0 por defecto
	private int porcentaje = 0; //porcentaje de la experiencia actual para llegar al sig nivel...
	private int pDificil = 0, pNormal = 0, pFacil = 0;

	/// <summary>
	/// Este metodo se encarga de determinar el nivel actual del usuario atraves de la experiencia
	/// obtenida, esto para evitar tener un campo 'nivel' en la BD que podria ser obviamente derivado
	/// de la experiencia.
	/// </summary>
	/// <param name="experiencia">
	/// Este parametro es utilizado practicamente para determinar el nivel actual en que el usuario
	/// se encuentra, su porcentaje y la experiencia maxima calculada.
	/// </param>
	/// <returns>
	/// Devuelve [Nivel, Porcentaje, EXP maxima]
	/// </returns>
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

	/// <summary>
	/// Este metodo es la manera en que se decidio manejar los puntos obtenidos, de acuerdo
	/// al nivel y la dificultad de la pregunta, en resumen la manera de calcular los puntos es la
	/// siguiente:
	/// -- Los puntos de experiencia tienen un puntaje por defecto de 10 (siendo este el maximo)
	///    y su valor se vera reducido de acuerdo al nivel y la dificultad de la pregunta
	/// -- Los puntos de clasificacion tienen un puntaje por defecto de 1; el jugador debera tener
	///    un determinado numero de aciertos (acumulable) para que pueda obtener este punto, sin
	///    embargo mientras mayor sea la dificultad de la pregunta (y sea respondida correctamente)
	///    menos aciertos requerira para obtener el punto de clasificacion.
	/// </summary>
	/// <param name="nivel">Se recibe el nivel actual del jugador</param>
	/// <param name="dificultad">Se recibe la dificultad de las preguntas a las que tiene acceso
	/// </param>
	/// <returns>
	/// Devuelve [puntos experiencia, puntos(clasificacion)]
	/// </returns>
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
			case "Maestro":
				infoPuntaje[0] = 10;
				infoPuntaje[1] = 1; //punto (clasificacion)
				return infoPuntaje;
		}
		return infoPuntaje;
	}
}