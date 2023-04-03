# Assisted Terminal
Have you ever not known what command you need to type? Well assisted terminal is here to help you.
Assisted terminal allows you to enter normal bash commands but also plain text that will be converted into a command 
and executed. This is useful for when you don't know the exact command you need to type but you know what you want to do.

## WARNINGS (READ BEFORE USE)
- When typing plain text the command that GPT generates will be executed immediately without user confirmation.
- All terminal output and input will be submitted to OpenAI.
- I am not liable for any commands that are executed by this program.

## Limitations
- Currently only supports bash on Linux. (Bash must be in `/bin/bash`)
- Does not support interactive commands (e.g. vim, nano, etc. This includes commands that require user input such as apt without -y)

## Usage
Set the following environment variables:
- `OPENAI_API_KEY`: Your OpenAI API key
- `OPENAI_MODEL`: The model you want to use. Defaults to `GPT4`

You can then run the program.

If you want to execute a command then prefix your command with `/`. For example, to run `ls` you would type `/ls`.

If you want to execute a command using plain text then type the plain text. For example, 
to run `ls` you would type `list files` and the most likely output would be `ls`.

If you want to be root then run the program as root. **WARNING: This will allow the AI to run commands as root. Use with care.**
