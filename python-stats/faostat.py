from csv import reader
import json
from json import JSONEncoder

class Stat:
  country = ''
  percentage = 0

  def __init__(self, country, percentage):
    self.country = country
    self.percentage = percentage

  def __lt__(self, other):
    return self.percentage < other.percentage

  def __str__(self):
    return "{{{}' : {}}}".format(self.country, self.percentage)

  def __repr__(self):
    return str(self)

# The CSV file is :
# [0] : Country
# [1] : Crop
# [2] : Quantity

all_stats = []
all_lines = []

def get_unique_crops(all_lines):
  unique_crops = []

  for line in all_lines:
    if line[1] not in unique_crops:
      unique_crops.append(line[1])

  return unique_crops

def get_all_lines_with_crop(crop, all_lines):
  lines_crop = []

  for line in all_lines:
    if line[1] == crop and len(line[2]) > 0 and line[0] != "China, mainland":
      lines_crop.append(line)

  return lines_crop

with open("faostat.csv", 'r') as file:
  csv_reader = reader(file)

  for line in csv_reader:
    all_lines.append(line)

  unique_crops = get_unique_crops(all_lines)

  for crop in unique_crops:
    print('Crop is ' + crop)
    crop_stats = []
    crop_stats_json = []
    lines_crop = get_all_lines_with_crop(crop, all_lines)

    total_quantity = 0

    for line_crop in lines_crop:
      total_quantity += int(line_crop[2])

    for line_crop in lines_crop:
      percentage = format((int(line_crop[2]) / total_quantity) * 100, '.2f')
      crop_stats.append(Stat(line_crop[0], float(percentage)))
    
    crop_stats.sort(reverse=True)

    for crop_stat in crop_stats:  
      crop_stats_json.append({crop_stat.country : crop_stat.percentage})

    print(crop_stats_json)

    all_stats.append({crop : crop_stats_json})

    # input()

with open("data.json",'w',encoding = 'utf-8') as f:
  f.write(json.dumps(all_stats, indent = 2, sort_keys=True))
