import pandas as pd
import numpy as np
import scipy.io as scp
import scipy.optimize as scy
import matplotlib.pyplot as plt
import copy
import math
import seaborn as sns
import json

def main():

    #-------------RECOGIDA DE DATOS----------------
    # df = {
    # 'x': [45, 37, 42, 35, 39],
    # 'y': [38, 31, 26, 28, 33],
    # 'z': [10, 15, 17, 21, 12]
    # }
 
    with open('PruebasMM1.json', encoding="utf-8") as user_files:
        file_contents = user_files.read()

    data = json.loads(file_contents)
    
    dic = { }

    #INICIALIZACION DATA: PUEDE DAR ERROR SI NO ESTE EL USUARIO, CAMBIAR POR OTRO QUE ESTE

    variableNames = []
    for j in data['User']["Este es mi usuario"]: 
        if(j != "userName" and j != "zzlastGameSaved" and j != "email"):
            dic[j] = []
            variableNames = np.concatenate((variableNames, [j]))


    for i in data['User']:
        for j in data['User'][i]:
            if(j != "userName" and j != "zzlastGameSaved" and j != "email"):
                dic[j] = np.concatenate((dic[j], [data['User'][i][j]]))
        
    # form dataframe
    df = pd.DataFrame(dic, columns=variableNames)

    #-------------MATRIZ DE CORRELACION----------------
    matrix = df.corr(method = 'pearson').round(2)  # The method of correlation
    
    #.............VISUALIZACION-----------------------
    sns.heatmap(matrix, annot=True, vmax=1, vmin=-1, center=0, cmap='vlag')
    plt.show()

main()