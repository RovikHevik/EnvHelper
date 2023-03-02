using System.Collections;
using Microsoft.Extensions.DependencyInjection;

namespace EnvHelper;

public static class WriteEnvToFrontApp
{
    
    const string CLIENT_APP_PREFIX = "REACT_APP_";
    
    public static IServiceCollection SetNodeEnv(this IServiceCollection builder, IDictionary envDictionary, string currentDirectory)
    {
        List<EnvModel> envList = new();
        foreach (DictionaryEntry envItem in envDictionary)
        {
            if (envItem.Key.ToString().Contains(CLIENT_APP_PREFIX))
            {
                envList.Add(new EnvModel
                {
                    Name = envItem.Key.ToString(),
                    Value = envItem.Value.ToString(),
                    isExist = false
                });
            }
        }
        
        string? clientAppDirectory = Directory.GetDirectories(currentDirectory)
                                        .FirstOrDefault(d => d.Contains("ClientApp"));
        string envFilePath = Path.Combine(clientAppDirectory, ".env");
        string[] lines = File.ReadAllLines(envFilePath);
        
        for(int i = 0; i < lines.Length; i++)
        {
            string line = lines[i];
            if (line.Contains("="))
            {
                string[] lineSplit = line.Split("=");
                string envName = lineSplit[0];
                string envValue = lineSplit[1];
                EnvModel? envModel = envList.FirstOrDefault(e => e.Name == envName);
                if (envModel != null)
                {
                    envModel.isExist = true;
                    lines[i] = SetEnvLine(envModel, envValue);
                }
            }
        }

        string newEnvFile = "";
        foreach (var env in envList)
        {
            newEnvFile += ($"{env.Name}={env.Value}\n");
        }
        
        File.WriteAllText(envFilePath, newEnvFile);

        return builder;
    }

    private static string SetEnvLine(EnvModel envModel, string? needValue)
    {
        if(needValue == envModel.Value)
        {
            return $"{envModel.Name}={envModel.Value}";
        }

        return $"{envModel.Name}={needValue}";
    }
}