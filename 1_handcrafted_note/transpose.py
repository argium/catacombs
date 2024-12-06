key = "INCOMPLETE"
key2 = "UNSOLVABLE"

clue = "SGHO EIUII ABD MOO NK GHN, EDNY, DL LLSE"
clue_reverse = "ESLL LD ,YNDE ,NHG KN OOM DBA IIUIE OHGS"

def get_sequence(key):
  sorted = [c for c in key]
  sorted.sort()

  # zip list
  sorted = dict(zip(sorted, range(1, len(key)+1)))
  return [sorted[c] for c in key]

def get_table(plaintext, count):
  # split by count
  return [plaintext[i:i+count] for i in range(0, len(plaintext), count)]


def printall(key, sequence, table):
  for c in key:
    print(f"{c:<3}", end='')
  print()

  for c in sequence:
    print(f"{c:<3}", end='')
  print()

  for row in table:
    for c in row:
      print(f"{c:<3}", end='')
    print()  
  print()

def print_table(table):
  for row in table:
    for c in row:
      print(f"{c:<3}", end='')
    print()  
  print()

def transpose(table):
  table.sort(key=lambda x: )
  new = []
  for row in table:
    new_row = []
  for i in range(len(table)):


  
sequence = get_sequence(key)
table = get_table(clue, len(key))
print(table)
c = [[c for c in key]] + [[c for c in sequence]] + table
print_table(c)
# printall(key, sequence, table)
