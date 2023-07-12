# Dependencies
from flask import Flask, request, jsonify
import joblib
import traceback
import pandas as pd
import numpy as np
import sys
# Your API definition
app = Flask(__name__)

@app.route('/predict', methods=['POST'])
def predict():
    if lr:
        try:
            print("hello0")
            
            json_ = request.json
            print("hello1")
            print(json_)
            print("hello2")
            query = pd.get_dummies(pd.DataFrame(json_))
            print("hello3")
            query = query.reindex(columns=model_columns, fill_value=0)

            prediction = list(lr.predict(query))
            
            return jsonify({'prediction': str(prediction)})

        except:
            print("error")
            return jsonify({'traceabdo': traceback.format_exc()})
    else:
        print ('Train the model first')
        return ('No model here to use')

if __name__ == '__main__':
    try:
        port = int(sys.argv[1]) # This is for a command-line input
    except:
        port = 12345 # If you don't provide any port the port will be set to 12345

    lr = joblib.load("model.pkl") # Load "model.pkl"
    print ('Model loaded')
    model_columns = joblib.load("model_columns.pkl") # Load "model_columns.pkl"
    print ('Model columns loaded')

    app.run(port=port, debug=True)