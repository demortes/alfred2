const { writeFileSync, readFileSync } = require('fs');
const { join } = require('path');
const { config } = require('dotenv');

// Using require('colors') was causing issues with TypeScript's type system.
// The colors are not essential for the script to run, so they have been removed.

interface Environment {
    appVersion: string;
    production: boolean;
    apiUrl: string;
    twitch_client_id: string;
}

const setEnv = () => {
    // Load environment variables from .env file, falling back to .env.example
    let result = config({ path: join(__dirname, '.env') });
    if (result.error) {
        // If .env is missing, try .env.example
        const tryExample = config({ path: join(__dirname, '.env.example') });
        if (!tryExample.error) {
            result = tryExample;
            console.warn('Loaded environment from .env.example (fallback).');
        } else {
            // Neither .env nor .env.example found — continue but warn instead of exiting
            console.warn('Warning: No .env or .env.example file found. Environment variables may be missing.');
        }
    }

    // Get package version
    const packageJsonPath = join(__dirname, '../../package.json');
    const packageJson = JSON.parse(readFileSync(packageJsonPath, 'utf-8'));
    const appVersion = packageJson.version;

    // Validate required environment variables
    const requiredEnvVars = [
        'API_URL',
        'TWITCH_CLIENT_ID'
    ];

    const missingVars = requiredEnvVars.filter(varName => !process.env[varName]);
    if (missingVars.length > 0) {
        console.warn('Missing required environment variables:', missingVars.join(', '));
        console.warn('Continuing without required env vars; generated environment.ts may contain empty values.');
    }

    // Configure target path for environment file
    const targetPath = join(__dirname, 'environment.ts');

    // Create environment config
    const environment: Environment = {
        appVersion,
        production: process.env['PRODUCTION'] === 'true',
        apiUrl: process.env['API_URL']!,
        twitch_client_id: process.env['TWITCH_CLIENT_ID']!
    };

    // Generate environment.ts content
    const envConfigFile = `export const environment = ${JSON.stringify(environment, null, 2)};
`;
    // Write the environment file
    console.log('The file `environment.ts` will be written with the following content: \n');
    console.log(envConfigFile);

    try {
        writeFileSync(targetPath, envConfigFile);
        console.log(`Angular environment.ts file generated correctly at ${targetPath} \n`);
    } catch (err) {
        console.error('Error writing environment.ts file:', err);
        process.exit(1);
    }
};

setEnv();