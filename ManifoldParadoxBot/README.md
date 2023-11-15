## WIP

# Development

1. Clone this repository.
2. Set the `GOOGLE_APPLICATIONS_CREDENTIALS` environment variable. The value must be the filepath to your firebase private key file ("path/to/serviceAccountKey.json").
3. Set the discord bot token as the first program argument if you want to run it from your IDE.
4. Compile and build.
5. Start the program passing the discord bot token as the first argument. Example from windows cli: `ManifoldParadoxBot.exe YOUR_DISCORD_BOT_TOKEN_LONG_STRING`

# DOCKER IMAGE | Windows

1. Clone this repository.
2. Clone the `Dockerfile.example` file and rename it to `Dockerfile`.
3. Fill the `GOOGLE_APPLICATIONS_CREDENTIALS` field in the newly created Dockerfile. The value must be the name of your firebase private key file ("serviceAccountKey.json").
4. Make sure you are in the solution path in your terminal. `X:\YOUR_PATH_TO_SOLUTION_FOLDER\`
5. Build your image by running: `docker build -t YOUR_IMAGE_NAME .`.
6. Run your docker image with the following command: `docker run YOUR_IMAGE_NAME YOUR_DISCORD_BOT_TOKEN`, by replacing "YOUR_DISCORD_BOT_TOKEN" with your actual discord token.
7. The application should be successfully running by now.