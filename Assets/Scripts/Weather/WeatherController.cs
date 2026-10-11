using System;
using UnityEngine;

public enum Season { Spring, Summer, Autumn, Winter }
public enum WeatherType { Clear, Cloudy, Rain, Snow, Storm }

/// <summary>Game calendar, clock, seasonal weather and day/night lighting.</summary>
[DisallowMultipleComponent]
public sealed class WeatherController : MonoBehaviour
{
    [Header("Clock and calendar")]
    [SerializeField, Min(1)] private int year = 1;
    [SerializeField, Range(1, 12)] private int month = 3;
    [SerializeField, Range(1, 31)] private int day = 1;
    [SerializeField, Range(0f, 23.99f)] private float startingHour = 8f;
    [Tooltip("Real minutes needed for one complete game day.")]
    [SerializeField, Min(0.1f)] private float minutesPerDay = 20f;
    [SerializeField] private bool clockPaused;
    [SerializeField] private bool useUnscaledTime;

    [Header("Day and night")]
    [SerializeField] private Light sun;
    [SerializeField] private Light moon;
    [SerializeField, Range(0f, 12f)] private float sunriseHour = 6f;
    [SerializeField, Range(12f, 24f)] private float sunsetHour = 18f;
    [SerializeField, Min(0f)] private float daylightIntensity = 1.2f;
    [SerializeField, Min(0f)] private float moonlightIntensity = 0.15f;
    [SerializeField] private float sunHeading = -30f;
    [SerializeField] private Color daylightColor = new Color(1f, 0.95f, 0.85f);
    [SerializeField] private Color twilightColor = new Color(1f, 0.48f, 0.25f);
    [Tooltip("Controls the scene's global ambient intensity while enabled.")]
    [SerializeField] private bool controlAmbientLight = true;
    [SerializeField, Min(0f)] private float dayAmbientIntensity = 1f;
    [SerializeField, Min(0f)] private float nightAmbientIntensity = 0.15f;

    [Header("Weather")]
    [SerializeField] private WeatherType weather = WeatherType.Clear;
    [SerializeField] private bool automaticWeather = true;
    [SerializeField, Min(0.1f)] private float weatherIntervalHours = 6f;
    [SerializeField, Min(0.1f)] private float transitionSeconds = 3f;
    [Tooltip("Assign the existing rain Particle System from the Hierarchy.")]
    [SerializeField] private ParticleSystem rainParticles;
    [SerializeField] private ParticleSystem snowParticles;
    [SerializeField, Min(0f)] private float rainRate = 1800f;
    [SerializeField, Min(0f)] private float snowRate = 500f;

    private DateTime currentDateTime;
    private double hoursUntilWeatherChange;
    private float rainAmount;
    private float snowAmount;
    private float cloudAmount;
    private float originalAmbientIntensity;
    private bool initialized;

    public DateTime CurrentDateTime => currentDateTime;
    public int Year => currentDateTime.Year;
    public int Month => currentDateTime.Month;
    public int Day => currentDateTime.Day;
    public int Hour => currentDateTime.Hour;
    public int Minute => currentDateTime.Minute;
    public string ClockText => currentDateTime.ToString("HH:mm");
    public Season CurrentSeason => GetSeason(Month);
    public WeatherType CurrentWeather => weather;
    public bool IsDay => currentDateTime.TimeOfDay.TotalHours >= sunriseHour
                         && currentDateTime.TimeOfDay.TotalHours < sunsetHour;
    public bool ClockPaused { get => clockPaused; set => clockPaused = value; }

    public event Action<DateTime> TimeChanged;
    public event Action<Season> SeasonChanged;
    public event Action<WeatherType> WeatherChanged;

    private void Awake()
    {
        ValidateSettings();
        currentDateTime = new DateTime(year, month, day).AddHours(startingHour);
        hoursUntilWeatherChange = weatherIntervalHours;
        initialized = true;
    }

    private void OnEnable()
    {
        originalAmbientIntensity = RenderSettings.ambientIntensity;
        if (!initialized) return;
        UpdateWeatherVisuals(0f, true);
        UpdateLighting();
    }

    private void Update()
    {
        float delta = useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
        if (!clockPaused)
        {
            double gameHours = delta * 24d / (Mathf.Max(0.1f, minutesPerDay) * 60d);
            AdvanceHours(gameHours);
            if (automaticWeather)
            {
                hoursUntilWeatherChange -= gameHours;
                if (hoursUntilWeatherChange <= 0d)
                {
                    RandomizeWeather();
                    hoursUntilWeatherChange = Math.Max(0.1d, weatherIntervalHours);
                }
            }
        }
        UpdateWeatherVisuals(delta, false);
        UpdateLighting();
    }

    /// <summary>Sets a valid calendar date and clock time, including leap years.</summary>
    public void SetDateTime(int newYear, int newMonth, int newDay, int hour = 8, int minute = 0)
    {
        DateTime next = new DateTime(newYear, newMonth, newDay, hour, minute, 0);
        ApplyDateTime(next);
        hoursUntilWeatherChange = Math.Max(0.1d, weatherIntervalHours);
        UpdateLighting();
    }

    public void SetTime(int hour, int minute = 0) => SetDateTime(Year, Month, Day, hour, minute);

    public void AdvanceHours(double hours)
    {
        if (double.IsNaN(hours) || double.IsInfinity(hours))
            throw new ArgumentOutOfRangeException(nameof(hours));
        // Clamp at DateTime boundaries instead of overflowing after long sessions.
        double available = hours >= 0d
            ? (DateTime.MaxValue - currentDateTime).TotalHours
            : (currentDateTime - DateTime.MinValue).TotalHours;
        DateTime next = Math.Abs(hours) >= available
            ? (hours >= 0d ? DateTime.MaxValue : DateTime.MinValue)
            : currentDateTime.AddHours(hours);
        ApplyDateTime(next);
    }

    private void ApplyDateTime(DateTime next)
    {
        Season previousSeason = CurrentSeason;
        long previousMinute = currentDateTime.Ticks / TimeSpan.TicksPerMinute;
        currentDateTime = next;
        if (previousSeason != CurrentSeason) SeasonChanged?.Invoke(CurrentSeason);
        if (previousMinute != next.Ticks / TimeSpan.TicksPerMinute) TimeChanged?.Invoke(next);
    }

    public void SetWeather(WeatherType next)
    {
        if (!Enum.IsDefined(typeof(WeatherType), next))
            throw new ArgumentOutOfRangeException(nameof(next));
        if (weather == next) return;
        weather = next;
        WeatherChanged?.Invoke(weather);
    }

    public void SetAutomaticWeather(bool enabled)
    {
        automaticWeather = enabled;
        hoursUntilWeatherChange = Math.Max(0.1d, weatherIntervalHours);
    }

    [ContextMenu("Randomize Weather")]
    public void RandomizeWeather()
    {
        float roll = UnityEngine.Random.value;
        switch (CurrentSeason)
        {
            case Season.Spring:
                SetWeather(roll < 0.35f ? WeatherType.Clear : roll < 0.6f ? WeatherType.Cloudy : WeatherType.Rain);
                break;
            case Season.Summer:
                SetWeather(roll < 0.65f ? WeatherType.Clear : roll < 0.8f ? WeatherType.Cloudy : roll < 0.95f ? WeatherType.Rain : WeatherType.Storm);
                break;
            case Season.Autumn:
                SetWeather(roll < 0.25f ? WeatherType.Clear : roll < 0.65f ? WeatherType.Cloudy : WeatherType.Rain);
                break;
            default:
                SetWeather(roll < 0.25f ? WeatherType.Clear : roll < 0.55f ? WeatherType.Cloudy : WeatherType.Snow);
                break;
        }
    }

    // Northern hemisphere: March-May, June-August, September-November, December-February.
    private static Season GetSeason(int calendarMonth)
    {
        if (calendarMonth >= 3 && calendarMonth <= 5) return Season.Spring;
        if (calendarMonth >= 6 && calendarMonth <= 8) return Season.Summer;
        if (calendarMonth >= 9 && calendarMonth <= 11) return Season.Autumn;
        return Season.Winter;
    }

    private void UpdateWeatherVisuals(float delta, bool immediate)
    {
        float step = immediate ? float.MaxValue : delta / Mathf.Max(0.1f, transitionSeconds);
        float targetRain = weather == WeatherType.Storm ? 1.6f : weather == WeatherType.Rain ? 1f : 0f;
        float targetCloud = weather == WeatherType.Clear ? 0f : weather == WeatherType.Storm ? 1f : 0.55f;
        rainAmount = Mathf.MoveTowards(rainAmount, targetRain, step);
        snowAmount = Mathf.MoveTowards(snowAmount, weather == WeatherType.Snow ? 1f : 0f, step);
        cloudAmount = Mathf.MoveTowards(cloudAmount, targetCloud, step);
        SetEmission(rainParticles, rainAmount * rainRate);
        SetEmission(snowParticles, snowAmount * snowRate);
    }

    private static void SetEmission(ParticleSystem particles, float rate)
    {
        if (particles == null) return;
        var emission = particles.emission;
        emission.enabled = true;
        emission.rateOverTime = rate;
        if (rate > 0f && !particles.isPlaying) particles.Play();
        else if (rate <= 0f && particles.isPlaying)
            particles.Stop(true, ParticleSystemStopBehavior.StopEmitting);
    }

    private void UpdateLighting()
    {
        float hour = (float)currentDateTime.TimeOfDay.TotalHours;
        float dayLength = Mathf.Max(0.01f, sunsetHour - sunriseHour);
        float phase = IsDay ? (hour - sunriseHour) / dayLength
            : 1f + Mathf.Repeat(hour - sunsetHour, 24f) / Mathf.Max(0.01f, 24f - dayLength);
        float elevation = Mathf.Sin(phase * Mathf.PI);
        float daylight = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(elevation * 3f));
        float weatherDim = Mathf.Lerp(1f, 0.4f, cloudAmount);
        if (sun != null)
        {
            sun.transform.rotation = Quaternion.Euler(phase * 180f, sunHeading, 0f);
            sun.intensity = daylightIntensity * daylight * weatherDim;
            sun.color = Color.Lerp(twilightColor, daylightColor, daylight);
        }
        if (moon != null)
        {
            moon.transform.rotation = Quaternion.Euler(phase * 180f + 180f, sunHeading, 0f);
            moon.intensity = moonlightIntensity * Mathf.Clamp01(-elevation * 3f) * weatherDim;
        }
        if (controlAmbientLight)
            RenderSettings.ambientIntensity = Mathf.Lerp(nightAmbientIntensity, dayAmbientIntensity, daylight) * weatherDim;
    }

    private void OnDisable()
    {
        if (controlAmbientLight) RenderSettings.ambientIntensity = originalAmbientIntensity;
        if (rainParticles != null) rainParticles.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        if (snowParticles != null) snowParticles.Stop(true, ParticleSystemStopBehavior.StopEmitting);
    }

    private void OnValidate() => ValidateSettings();

    private void ValidateSettings()
    {
        year = Mathf.Clamp(year, 1, 9999);
        month = Mathf.Clamp(month, 1, 12);
        day = Mathf.Clamp(day, 1, DateTime.DaysInMonth(year, month));
        startingHour = Mathf.Clamp(startingHour, 0f, 23.99f);
        minutesPerDay = Mathf.Max(0.1f, minutesPerDay);
        sunriseHour = Mathf.Clamp(sunriseHour, 0f, 11.99f);
        sunsetHour = Mathf.Clamp(sunsetHour, 12f, 23.99f);
        weatherIntervalHours = Mathf.Max(0.1f, weatherIntervalHours);
        transitionSeconds = Mathf.Max(0.1f, transitionSeconds);
    }
}
