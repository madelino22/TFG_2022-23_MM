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
def Firebase(dic, data, variableNames):
    # Guardamos el nombre de las variables
    variableNames = ["None", "Duelist", "Sniper", "Healer", "NonePartner", "DuelistPartner", "SniperPartner", "HealerPartner", "Won", "Fun"]
   
    index = 0

    # Guardamos los datos
    for p in data['Matches']: #Partida0, 1, 2, etc
        team0 = [] # Red
        team1 = [] # Blue
  
        result = data['Matches'][p]['pointsRed'] - data['Matches'][p]['pointsBlue']
        winner_team = -1 # draw
        if result > 0: # Red won
            winner_team = 0
        elif result < 0: # Blue won
            winner_team = 1

        # Guardamos nombres de integrantes en cada equipo
        for u in data['Matches'][p]:
            if data['Matches'][p][u]['t'] == 0:
                team0 = np.concatenate((team0, [data['Matches'][p][u]['name']]))
            else:
                team1 = np.concatenate((team1, [data['Matches'][p][u]['name']]))
 
        team_mate = 0
        for u in team0:
            dic[u] = []
            for v in variableNames:
                dic[index] = np.concatenate((dic[u], [0]))
            
            role = data['Matches'][p][u]['lastRole']
            dic[index][role] = 1
            other = team0[team_mate % 2]
            partner_role = data['Matches'][p][other]['lastRole'] + 'Partner'
            dic[index][partner_role] = 1
            dic[index]["Fun"] = data['Matches'][p][u]['gameRating'] 
            if winner_team == data['Matches'][p][u]['t']:
                dic[index]["Won"] = 1
            team_mate += 1
            index += 1

        team_mate = 0
        for u in team1:
            dic[u] = []
            for v in variableNames:
                dic[index] = np.concatenate((dic[u], [0]))
            
            role = data['Matches'][p][u]['lastRole']
            dic[index][role] = 1
            other = team1[team_mate % 2]
            partner_role = data['Matches'][p][other]['lastRole'] + 'Partner'
            dic[index][partner_role] = 1
            dic[index]["Fun"] = data['Matches'][p][u]['gameRating'] 
            if winner_team == data['Matches'][p][u]['t']:
                dic[index]["Won"] = 1
            team_mate += 1
            index += 1

    return dic, variableNames

# MAIN==================================================================================
def main():
    firebase_file = 'Firebase/PruebasMM4.json'
    
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
    variableNames = [] # Guardamos los nombres de las variables de la tabla (RESPETAR EL ORDEN!!!!) 

    # PARTE 1:==============================================================================
    #Añadir datos del Firebase al diccionario
    dic, variableNames = Firebase(dic, data, variableNames)

    # form dataframe
    df = pd.DataFrame(dic, columns=variableNames)

    #-------------MATRIZ DE CORRELACION----------------
    matrix = df.corr(method = 'pearson').round(2)  # The method of correlation
    
    #.............VISUALIZACION-----------------------
    sns.heatmap(matrix, annot=True, vmax=1, vmin=-1, center=0, cmap='vlag')
    plt.show()

main()