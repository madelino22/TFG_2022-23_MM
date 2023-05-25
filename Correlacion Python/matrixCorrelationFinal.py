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

    # Guardamos los datos
    for p in data['Matches']: #Partida0, 1, 2, etc  
        result = data['Matches'][p]['pointsRed'] - data['Matches'][p]['pointsBlue']
        winner_team = -1 # draw
        if result > 0: # Red won
            winner_team = 0
        elif result < 0: # Blue won
            winner_team = 1

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
                        dic[v] = np.concatenate((dic[v], [data['Matches'][p][u][v]]))
                    elif v == 'Won':
                        if winner_team == data['Matches'][p][u]['t']:
                            dic['Won'] =  np.concatenate((dic['Won'], [1]))
                            dic['Draw'] = np.concatenate((dic['Draw'], [0]))
                            dic['Lost'] =  np.concatenate((dic['Lost'], [0]))
                        elif winner_team == -1:
                            dic['Draw'] = np.concatenate((dic['Draw'], [1]))
                            dic['Won'] =  np.concatenate((dic['Won'], [0]))
                            dic['Lost'] =  np.concatenate((dic['Lost'], [0]))
                        else:
                            dic['Lost'] =  np.concatenate((dic['Lost'], [1]))
                            dic['Won'] =  np.concatenate((dic['Won'], [0]))
                            dic['Draw'] = np.concatenate((dic['Draw'], [0]))
      

    return dic, variableNames, user_names


# PARTE 2:==============================================================================
def BigFive(dic, user_names, file_name, variableNames): #'a.csv'
    "PROCESAMOS EL CSV DE BIG FIVE"

    # Guardamos el nombre de las variables
    variableNames = np.concatenate((variableNames, ["Extraversion", "Amabilidad", "Responsabilidad", "Apertura", "Neuroticismo"]))
    dic["Extraversion"] = []
    dic["Amabilidad"] = []
    dic["Responsabilidad"] = []
    dic["Apertura"] = []
    dic["Neuroticismo"] = []

    with open(file_name, newline='', encoding='utf-8') as archivo_csv: #sin el utf-8 petaba
        lector_csv = csv.reader(archivo_csv)
        next(lector_csv)

        # RECORREMOS LOS USARIOS DE FIREBASE
        for user_name in user_names:
            encontrado = False

            # BUSCAR FILA EN CSV
            for fila in lector_csv:
                nombre = fila[1]
                nombre = CorrectString(nombre)

                if nombre == user_name:
                    encontrado = True
                    ex = fila[5]
                    ex2=fila[7]
                    ex3=fila[9]
                    ex4=fila[11]
                    ex5=fila[13]
                    #print(nombre, ex, ex2, ex3, ex4, ex5)
                    res=int(ex)
                    res2=int(ex2)
                    res3=int(ex3)
                    res4=int(ex4)
                    res5=int(ex5)

                    extraversion = res/12*100
                    amabilidad = (res2/12)*100
                    responsabilidad = (res3/12) * 100
                    apertura = (res4/12) * 100
                    neuroticismo = (res5/12)*100

                    dic["Extraversion"] = np.concatenate((dic["Extraversion"], [extraversion]))
                    dic["Amabilidad"] = np.concatenate((dic["Amabilidad"], [amabilidad]))
                    dic["Responsabilidad"] =  np.concatenate((dic["Responsabilidad"], [responsabilidad]))
                    dic["Apertura"] = np.concatenate((dic["Apertura"], [apertura]))
                    dic["Neuroticismo"] = np.concatenate((dic["Neuroticismo"], [neuroticismo]))
                    archivo_csv.seek(0) # Volvemos al principio del archivo
                    break

            if encontrado == False:
                print("ERROR: Nombre " + user_name + " no existe en Big Five.")

            archivo_csv.seek(0) # Volvemos al principio del archivo
    return dic, variableNames

# MAIN==================================================================================
def main():
    firebase_file = 'Firebase/PruebasMM6.json'
    big_five_file = 'BigFivePruebaFinal.csv'
    
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

    # PARTE 2:==============================================================================
    # Añadir los datos de Big Five al dicionario
    dic, variableNames = BigFive(dic, user_names, big_five_file, variableNames)

    print(5)
    # form dataframe
    df = pd.DataFrame(dic, columns=variableNames) # EMPTY DATAFRAME???????????'

    #-------------MATRIZ DE CORRELACION----------------
    matrix = df.corr(method = 'pearson').round(2)  # The method of correlation
    
    #.............VISUALIZACION-----------------------
    sns.heatmap(matrix, annot=True, vmax=1, vmin=-1, center=0, cmap='vlag')
    plt.show()

main()