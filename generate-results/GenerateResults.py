# %%
import requests
import statistics
import os
import json
import matplotlib.pyplot as plt
import numpy as np
# if using a Jupyter notebook, includue:
%matplotlib inline



# %%
UPDATE_LOCAL_COPY = False

output_path = "feedback.json"

if((not os.path.exists(output_path)) or UPDATE_LOCAL_COPY):
    dataset_url = "https://vaerio-level-providor.herokuapp.com/feedback"

    resp = requests.get(dataset_url)
    feedbacks_json = resp.json()
    with open(output_path, "w") as f:
        f.write(json.dumps(feedbacks_json))
else:
    with open(output_path, "r") as f:
        feedbacks_json = json.loads(f.read())

#filter out all feedbacks before system was stable
#feedbacks_json = [feedback_json for feedback_json in feedbacks_json["feedbackItems"] if feedback_json["timestamp"] >= 1625781787]
# filter to friday morning
feedbacks_json = [feedback_json for feedback_json in feedbacks_json["feedbackItems"] if feedback_json["timestamp"] >= 1625817219]


# %%
feedbacks_by_player_ids = {}
for feedback_json in feedbacks_json:
    if feedback_json["player_id"] not in feedbacks_by_player_ids:
        feedbacks_by_player_ids[feedback_json["player_id"]] = []
    feedbacks_by_player_ids[feedback_json["player_id"]].append(feedback_json)

# %%
num_feedbacks = [len(feedbacks_by_player_ids[player_id]) for player_id in feedbacks_by_player_ids]
print(statistics.mean(num_feedbacks))
print(statistics.median(num_feedbacks))
print(statistics.geometric_mean(num_feedbacks))
print()

# %%

for player_id in feedbacks_by_player_ids:
    feedbacks_by_player_ids[player_id] = sorted(feedbacks_by_player_ids[player_id], key=lambda feedback: feedback["timestamp"])

# %%
novelty_desired_vs_served_differences_by_player_id = {}
abs_novelty_desired_vs_served_differences_by_player_id = {}
for player_id, feedbacks_by_player_id in feedbacks_by_player_ids.items():
    if(len(feedbacks_by_player_id)<2):
        continue

    novelty_desired_vs_served_differences_by_player_id[player_id] = []
    abs_novelty_desired_vs_served_differences_by_player_id[player_id] = []

    for feedback_i in range(1, len(feedbacks_by_player_id)):
        novelty_difference = feedbacks_by_player_id[feedback_i]["rated_novelty"] - feedbacks_by_player_id[feedback_i-1]["desired_novelty"]
        novelty_desired_vs_served_differences_by_player_id[player_id].append(novelty_difference)
        abs_novelty_desired_vs_served_differences_by_player_id[player_id].append(abs(novelty_difference))

# %%

all_novelty_differences_abs = []
quality_surface_novelty_differences_abs = []
random_novelty_differences_abs = []



for player_id in abs_novelty_desired_vs_served_differences_by_player_id:
     for abs_novelty_desired_vs_served in abs_novelty_desired_vs_served_differences_by_player_id[player_id]:
        all_novelty_differences_abs.append(abs_novelty_desired_vs_served)
        
        if feedbacks_by_player_ids[player_id][0]["experiment_name"] == "enjoyment-surface":
            quality_surface_novelty_differences_abs.append(abs_novelty_desired_vs_served)
        
        if feedbacks_by_player_ids[player_id][0]["experiment_name"] == "Random Level Generator":
            random_novelty_differences_abs.append(abs_novelty_desired_vs_served)
        

#%%

print(len(all_novelty_differences_abs))
print(statistics.mean(all_novelty_differences_abs))
print(statistics.median(all_novelty_differences_abs))
print()

# %%

print("Quality Surface - Difference in Desired Vs Served Novelty")
print("Number of Feedbacks:",  len(quality_surface_novelty_differences_abs))
print("Mean:", statistics.mean(quality_surface_novelty_differences_abs))
print("Median:", statistics.median(quality_surface_novelty_differences_abs))
print("Number of feedbacks hitting exactly the desired novelty:", len([value for value in quality_surface_novelty_differences_abs if value == 0]) / len(quality_surface_novelty_differences_abs))

print()



# %%

print("Random - Difference in Desired Vs Served Novelty")
print("Number of Feedbacks:", len(random_novelty_differences_abs))
print("Mean:", statistics.mean(random_novelty_differences_abs))
print("Median:", statistics.median(random_novelty_differences_abs))
print("Number of feedbacks hitting exactly the desired novelty:", len([value for value in random_novelty_differences_abs if value == 0]) / len(random_novelty_differences_abs))

print()

# %%

plt.hist(quality_surface_novelty_differences_abs, bins=[0, 1, 2, 3, 4, 5])
plt.ylim(0, 200)
# %%

plt.hist(random_novelty_differences_abs, bins=[0, 1, 2, 3, 4, 5])
plt.ylim(0, 200)
# %%

rated_enjoyment_by_player_id = {}
for player_id in feedbacks_by_player_ids:
    rated_enjoyment_by_player_id[player_id] = [feedback["enjoyment"] for feedback in feedbacks_by_player_ids[player_id]]

# %%

all_enjoyments = []
quality_surface_enjoyments = []
random_enjoyments = []

for player_id, enjoyments in rated_enjoyment_by_player_id.items():
        all_enjoyments += enjoyments
        
        if feedbacks_by_player_ids[player_id][0]["experiment_name"] == "enjoyment-surface":
            quality_surface_enjoyments += enjoyments
        
        if feedbacks_by_player_ids[player_id][0]["experiment_name"] == "Random Level Generator":
            random_enjoyments += enjoyments 
# %%


print(len(all_enjoyments))
print(statistics.mean(all_enjoyments))
print(statistics.median(all_enjoyments))
print()

# %%

print("Quality Surface - Enjoyment")
print("Number of Feedbacks:",  len(quality_surface_enjoyments))
print("Mean:", statistics.mean(quality_surface_enjoyments))
print("Median:", statistics.median(quality_surface_enjoyments))

print()



# %%

print("Random - Enjoyment")
print("Number of Feedbacks:", len(random_enjoyments))
print("Mean:", statistics.mean(random_enjoyments))
print("Median:", statistics.median(random_enjoyments))

print()
# %%
