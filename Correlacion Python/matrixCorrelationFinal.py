import pandas as pd
import numpy as np
import scipy.io as scp
import scipy.optimize as scy
import matplotlib.pyplot as plt
import copy
import math
import seaborn as sns
import json
import csv
from unidecode import unidecode # acentos

#PruebasMM6

def CorrectString(palabra):
    "Quita acentos, mayusculas, etc"
    palabra = palabra.lower() # quitar mayusculas
    palabra = palabra.replace("á", "a")
    palabra = palabra.replace("é", "e")
    palabra = palabra.replace("í", "i")
    palabra = palabra.replace("ó", "o")
    palabra = palabra.replace("ú", "u")
    palabra = palabra.replace("ñ", "n")
    palabra = palabra.replace("ç", "c")
    palabra = palabra.replace(" ", "")
    palabra = palabra.strip() # quitar espacios
    return palabra

# PARTE 1:==============================================================================
def Firebase(dic, data, variableNames, user_names):    
    # Guardamos el nombre de las variables
    variableNames = ["damageInflicted", "damageReceived", "deaths", "gameRating", "healMe", "healOthers", "hitDistance", "kills", "totalShots", "totalShotsEnemy", "Won", "Draw", "Lost"]
    for v in variableNames:
        dic[v] = []

    index = 0
    # Guardamos los datos
    for p in data['Matches']: #Partida0, 1, 2, etc  
        result = data['Matches'][p]['pointsRed'] - data['Matches'][p]['pointsBlue']
        winner_team = -1 # draw
        if result > 0: # Red won
            winner_team = 0
        elif result < 0: # Blue won
            winner_team = 1


        # Default value
        for v in variableNames:
            dic[v] = np.concatenate((dic[v], [0])) # anyadimos una columna/team mas al diccionario

        # Guardamos nombres de integrantes en cada equipo
        for u in data['Matches'][p]:
            if u != 'pointsBlue' and u != 'pointsRed' and u != 'winningChancesBlue' and u != 'winningChancesRed' and u != 'winner' and u != 'damageDealtBlue' and u !=  'damageDealtRed':
                
                # GUARDAR NOMBRE
                new_name = data['Matches'][p][u]['name']
                new_name = CorrectString(new_name)
                user_names = np.concatenate((user_names, [new_name]))

                # GUARDAR DATOS
                for v in variableNames:
                    if v != 'Won' and v != 'Draw' and v != 'Lost':
                        dic[v][index] = data['Matches'][p][u][v]
                    else:
                        if winner_team == data['Matches'][p][u]['t']:
                            dic['Won'][index] = 1
                        elif winner_team == -1:
                            dic['Draw'][index] = 1
                        else:
                            dic['Lost'][index] = 1
        index += 1

    return dic, variableNames, user_names

# MAIN==================================================================================
def main():
    firebase_file = 'Firebase/PruebasMM6.json'
    
    #-------------RECOGIDA DE DATOS----------------
    # df = {
    # 'x': [45, 37, 42, 35, 39],
    # 'y': [38, 31, 26, 28, 33],
    # 'z': [10, 15, 17, 21, 12]
    # }
 
    with open(firebase_file, encoding="utf-8") as user_files:
        file_contents = user_files.read()

    data = json.loads(file_contents)
    
    dic = { } # cada fila es un usuario, cada columna una variable dic[usario] = [deaths, kills, ...]
    #GUARDAR NOMBRES DE USERS===============================================0
    user_names = []
    #VARIABLES    
    variableNames = [] # Guardamos los nombres de las variables de la tabla (RESPETAR EL ORDEN!!!!) 

    # PARTE 1:==============================================================================
    #Añadir datos del Firebase al diccionario
    dic, variableNames, user_names = Firebase(dic, data, variableNames, user_names)

    # form dataframe
    df = pd.DataFrame(dic, columns=variableNames) # EMPTY DATAFRAME???????????'

    #-------------MATRIZ DE CORRELACION----------------
    matrix = df.corr(method = 'pearson').round(2)  # The method of correlation
    
    #.............VISUALIZACION-----------------------
    sns.heatmap(matrix, annot=True, vmax=1, vmin=-1, center=0, cmap='vlag')
    plt.show()

main()