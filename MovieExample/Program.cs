using Azure.AI.OpenAI;
//using Microsoft.Agents.AI;
using System.Text.Json;

// Prep
var jsonWithMovies = await File.ReadAllTextAsync("Movies.json");

var movieDataForRag = JsonSerializer.Deserialize<Movie[]>(jsonWithMovies);

//var question = new(ChatRole.User)