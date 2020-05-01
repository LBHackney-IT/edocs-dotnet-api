# edocs-dotnet-api 🚀

## Configuration

web.config

- EDOCSAPIKEY - api key to access this api
- EDOCSUSERNAME
- EDOCSPASSWORD
- EDOCSLIBRARY
- EDOCSTEMPFILEPATH - Folder path used to temporarily store documents downloaded from edocs (temp files will be deleted when sent)
- EDOCSSERVERNAME - optional (will only be used when present)

## Build

- Build is automated using [github actions](https://github.com/features/actions) on commit

- Artifacts are available within the git actions build
![image](https://user-images.githubusercontent.com/22743709/80125543-b6140300-8589-11ea-882d-7d3936e60728.png)


## Deploy

- Create classic app pool within iis 
  - use an identity with permission to run com api libs & save files to temp folder
  - set the app pool "enable 32 bit application" to true in advanced setting
- Create app within iis
- Navigate to app folder (right click on app -> explore)
- Backup app folder (configuration is located in web.config)
- Download and extract Artifact folder to app folder 
- Reconfigure web.config
  - create temp folder for EDOCSTEMPFILEPATH if necessary
- Restart app pool in iis

## Troubleshooting

### Errors
- `-2147220409` error code is related to iis app pool identity permissions
- `Retrieving the COM class factory for component with CLSID {BAE80C14-D2AC-11D0-8384-00A0C92018F4} failed due to the following error: 80040154 Class not registered (Exception from HRESULT: 0x80040154 (REGDB_E_CLASSNOTREG)).` This error code is when the app fails to find the edocs library, this if often due to "enable 32 bit application" being to set to false on the app pool, please set to true if only 32bit edocs is installed.
