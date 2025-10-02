import { writeFileSync } from 'fs';
import { fileURLToPath } from 'url';
import { dirname, join } from 'path';
import { config } from 'dotenv';
import 'colors';

const __filename = fileURLToPath(import.meta.url);
const __dirname = dirname(__filename);

interface Environment {
    appVersion: string;
    production: boolean;
    apiUrl: string;
}

const setEnv = async () => {
    // Load environment variables from .env file
    const result = config({
        path: join(__dirname, '.env')
    });

    if (result.error) {
        console.error('Error loading .env file:'.red, result.error);
        process.exit(1);
    }

    // Get package version
    const packageJsonPath = join(__dirname, '../../package.json');
    const packageJson = JSON.parse(await import('fs/promises').then(fs => fs.readFile(packageJsonPath, 'utf-8')));
    const appVersion = packageJson.version;

    // Validate required environment variables
    const requiredEnvVars = [
        'API_URL'
    ];

    const missingVars = requiredEnvVars.filter(varName => !process.env[varName]);
    if (missingVars.length > 0) {
        console.error('Missing required environment variables:'.red, missingVars.join(', '));
        process.exit(1);
    }

    // Configure target path for environment file
    const targetPath = join(__dirname, 'environment.ts');

    // Create environment config
    const environment: Environment = {
        appVersion,
        production: process.env['PRODUCTION'] === 'true',
        apiUrl: process.env['API_URL']!
    };

    // Generate environment.ts content
    const envConfigFile = `export const environment = ${JSON.stringify(environment, null, 2)};
`;
    // Write the environment file
    console.log('The file `environment.ts` will be written with the following content: \n'.magenta);
    console.log(envConfigFile.grey);

    try {
        writeFileSync(targetPath, envConfigFile);
        console.log(`Angular environment.ts file generated correctly at ${targetPath} \n`.green);
    } catch (err) {
        console.error('Error writing environment.ts file:'.red, err);
        process.exit(1);
    }
};

setEnv();
