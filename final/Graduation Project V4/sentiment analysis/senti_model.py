import pandas as pd
from sklearn.feature_extraction.text import TfidfVectorizer
from sklearn.svm import LinearSVC
import nltk
nltk.download('punkt')
nltk.download('wordnet')
from nltk.stem import WordNetLemmatizer
from nltk.tokenize import word_tokenize
import joblib

# Load the IMDB dataset and read it into a pandas dataframe
df = pd.read_csv('IMDB_Dataset.csv')

# Process the text data using lemmatization and tokenization
def process_text(text):
    lemmatizer = WordNetLemmatizer()
    tokens = word_tokenize(text.lower().strip())
    lemmatized_tokens = [lemmatizer.lemmatize(token) for token in tokens]
    return ' '.join(lemmatized_tokens)

df['processed_review'] = df['review'].apply(process_text)

# Convert the text data into numerical features using TfidfVectorizer
vectorizer = TfidfVectorizer()
X_vectors = vectorizer.fit_transform(df['processed_review'])
y = df['sentiment']

# Train a LinearSVC model on the entire dataset
clf = LinearSVC()
clf.fit(X_vectors, y)

# Define a function to categorize the sentiment score as positive, negative, or neutral
def categorize_sentiment(score):
    if score >= 0.05:
        return 'positive'
    elif score <= -0.05:
        return 'negative'
    else:
        return 'neutral'

# Define an input text to analyze
#input_text = "The movie was great! I really enjoyed it."
#input_text = "The Estate was bad! I really feel sad to buy it."
input_text = "this person is a honest and amazing."

# Process the input text using lemmatization and tokenization
processed_input_text = process_text(input_text)

# Convert the processed input text into numerical features using TfidfVectorizer
input_vector = vectorizer.transform([processed_input_text])

# Predict the sentiment of the input text using the trained model
sentiment_score = clf.decision_function(input_vector)[0]
sentiment_category = categorize_sentiment(sentiment_score)

# Print the sentiment category for the input text
print("Input text: ", input_text)
print("Sentiment category: ", sentiment_category)

# Save the trained model using joblib
# joblib.dump(clf, 'sentiment_model.pkl')
# print("Model saved.")