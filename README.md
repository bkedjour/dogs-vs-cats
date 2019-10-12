# Dogs vs Cats

Dogs vs Cats uses Google Cloud Platform.

To run the app, you must first set up authentication by creating a service account and setting an environment variable.
Provide authentication credentials by setting the environment variable **GOOGLE_APPLICATION_CREDENTIALS**. Replace **[PATH]** with the file path of the JSON file that contains your service account key. This variable only applies to your current shell session, so if you open a new session, set the variable again.

```shell
export GOOGLE_APPLICATION_CREDENTIALS="[PATH]"
```
