import pandas as pd
import numpy as np
import matplotlib.pyplot as plt

# df = pd.read_csv ('output.csv', header = None)
# df.columns = ['Name', 'ELO', 'K']

# df.plot(x="Name", y=["ELO", "K"], kind="bar", figsize=(9, 8))


# plt.show()
# print(df)   



df = pd.read_csv ('output.csv', header = None)
df.columns = ['Name', 'ELO', 'K']

ax = df.plot(x="Name", y=["ELO", "K"], kind="bar", figsize=(9, 8))

s = df["K"]

for index in range((df.shape[0])):
  ax.text(index, s[index], s[index], size=8)


plt.show()
print(s)   