# Automatic Dell Display Brightness Manager (using sunlight radiation data as source)
This App uses the Dell Display Manager (ddm.exe), which is usuable with every modern Dell Monitor, to adjust the brightness dependent from the current sunlight.
The sunlight radiation data is fetched from an API called "Solcast" (https://solcast.com/). Where an API Key is necessary to fetch the radiation data forecast for a specific location. Alternatively you can adjust the project "SunRadiation.API" to use another API.

When you have multiple Dell Monitors, every display will be controlled in parallel.

# Usage
- Publish the project "Dell.BrightnessManager.App" and put the executable in you startup folder.
- Run the App once and close it again.
- Navigate to "C:\Users\\\<USERNAME>\AppData\Local\DellBrightnessManager" and adjust your settings to your preferences.
- Run the app again and you're done.