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
 
    with open('JSONTest.json') as user_files:
        file_contents = user_files.read()

    data = json.loads(file_contents)

    for i in data['User']:
        for j in data[i]:
            print(j)
        print("Siguiejte")
  
    
    # form dataframe
    df = pd.DataFrame(df, columns=['x', 'y', 'z'])

    #-------------MATRIZ DE CORRELACION----------------
    matrix = df.corr(method = 'pearson').round(3)  # The method of correlation
    
    #.............VISUALIZACION-----------------------
    sns.heatmap(matrix, annot=True, vmax=1, vmin=-1, center=0, cmap='vlag')
    plt.show()

main()