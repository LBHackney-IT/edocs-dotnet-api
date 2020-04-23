# edocs-dotnet-api

## Configuration

web.config

- EDOCSAPIKEY - api key to access this api
- EDOCSUSERNAME
- EDOCSPASSWORD
- EDOCSLIBRARY
- EDOCSTEMPFILEPATH - Folder path used to temporarily store documents downloaded from edocs (temp files will be deleted when sent)
- EDOCSSERVERNAME - optional

## Build

Build is automated using git actions on commit\
Artifacts are available within the git actions build

## Deploy

- Create classic app pool within iis 
  - use an identity with permission to run com api libs & save files to temp folder
- Create app within iis
- Navigate to app folder (right click on app -> explore)
- Backup app folder (configuration is located in web.config)
- Download and extract Artifact folder to app folder 
- Reconfigure web.config
  - create temp folder for EDOCSTEMPFILEPATH if necessary
- Restart app pool in iis

## Troubleshooting

-2147220409 error code related to iis app pool identity permissions

