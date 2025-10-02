const setEnv = () => {
    const fs = require('fs');
    const writeFile = fs.writeFile;
  // Configure Angular `environment.ts` file path
    const targetPath = './src/environments/environment.ts';
  // Load node modules
    const colors = require('colors');
    const appVersion = require('../../package.json').version;
    const result = require('dotenv').config({
      path: 'src/environments/.env'
    });
    if (result.error) {
      if (result.error.code === 'ENOENT') {
        // This is not a fatal error in CI environments
        console.log(colors.yellow('Skipping .env file load. File not found. Using environment variables.'));
      } else {
        console.error(colors.red('Error loading .env file'), result.error);
        process.exit(1);
      }
    }
  // `environment.ts` file structure
    const envConfigFile = `export const environment = {
    auth: {
        clientId: '${process.env["AZURE_CLIENT_ID"]}',
        authority: '${process.env["AZURE_AUTHORITY"]}',
        redirectUri: '${process.env["AZURE_REDIRECT_URI"]}'
    },
    appVersion: '${appVersion}',
    production: ${process.env["PRODUCTION"] === 'true'},
    apiUrl: '${process.env["API_URL"]}'
  };
  `;
    console.log(colors.magenta('The file `environment.ts` will be written with the following content: \n'));
    writeFile(targetPath, envConfigFile, (err: any) => {
      if (err) {
        console.error(err);
        throw err;
      } else {
        console.log(colors.magenta(`Angular environment.ts file generated correctly at ${targetPath} \n`));
      }
    });
  };
  
  setEnv();
  