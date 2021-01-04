using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Scripting;

public class CityDataProvider {
    public static CityNameData getCityNames(int count) {
        var json = Resources.Load<TextAsset>("world-cities-by-country");
        var countries = JsonConvert.DeserializeObject<List<Country>>(json.text);

        Country country = countries[UnityEngine.Random.Range(0, countries.Count)];
        string[] cityNames = country.cities.OrderBy(city => UnityEngine.Random.value).Take(count).Select(city => city.name).ToArray();

        return new CityNameData(country.name, cityNames);
    }
}

[Preserve]
public struct CityNameData {
    public string countryName;
    public string[] cityNames;

    public CityNameData(string countryName, string[] cityNames) {
        this.countryName = countryName;
        this.cityNames = cityNames;
    }
}

[Preserve]
public class Country {
    public string name;
    public List<CityName> cities;
}

[Preserve]
public class CityName {
    public string name;
    public string geonameid;
}
