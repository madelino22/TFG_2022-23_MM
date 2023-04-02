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

def Firebase(dic, data, user_names):
    for i in data['User']:
        if i != "Este es mi usuario": # NO USAMOS ESTE ES MI USUARIO POR DEFECTO
            for j in data['User'][i]:
                if(j != "userName" and j != "zzlastGameSaved" and j != "email"):
                    dic[j] = np.concatenate((dic[j], [data['User'][i][j]]))
                elif (j == "userName"):
                    new_name = data['User'][i][j]
                    new_name = CorrectString(new_name)
                    user_names = np.concatenate((user_names, [new_name]))
    return dic, user_names

def BigFive(dic, user_names, file_name): #'a.csv'
    "PROCESAMOS EL CSV DE BIG FIVE"

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
                    archivo_csv.seek(0)
                    break

            if encontrado == False:
                print("Nombre " + user_name + " no existe en Big Five.")

            archivo_csv.seek(0)
    return dic


def main():
    firebase_file = 'PruebasMM2.json'
    big_five_file = 'BigFivePrueba1.csv'
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
    user_names = []

    #INICIALIZACION DATA: PUEDE DAR ERROR SI NO ESTE EL USUARIO, CAMBIAR POR OTRO QUE ESTE
    variableNames = []
    for j in data['User']["Este es mi usuario"]: 
        if(j != "userName" and j != "zzlastGameSaved" and j != "email"):
            dic[j] = []
            variableNames = np.concatenate((variableNames, [j]))
    variableNames = np.concatenate((variableNames, ["Extraversion", "Amabilidad", "Responsabilidad", "Apertura", "Neuroticismo"]))

    dic["Extraversion"] = []
    dic["Amabilidad"] = []
    dic["Responsabilidad"] = []
    dic["Apertura"] = []
    dic["Neuroticismo"] = []

    #Pasar el JSON a un diccionario
    dic, user_names = Firebase(dic, data, user_names)
    
    # Añadir los datos de Big Five al dicionario
    dic = BigFive(dic, user_names, big_five_file)
        
    # form dataframe
    df = pd.DataFrame(dic, columns=variableNames)

    #-------------MATRIZ DE CORRELACION----------------
    matrix = df.corr(method = 'pearson').round(2)  # The method of correlation
    
    #.............VISUALIZACION-----------------------
    sns.heatmap(matrix, annot=True, vmax=1, vmin=-1, center=0, cmap='vlag')
    plt.show()

main()