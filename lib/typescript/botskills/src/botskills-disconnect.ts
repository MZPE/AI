/**
 * Copyright(c) Microsoft Corporation.All rights reserved.
 * Licensed under the MIT License.
 */

import * as program from 'commander';
import { existsSync } from 'fs';
import { extname, isAbsolute, join, resolve } from 'path';
import { disconnectSkill } from './functionality';
import { ConsoleLogger, ILogger} from './logger';
import { IDisconnectConfiguration } from './models';

function showErrorHelp(): void {
    program.outputHelp((str: string) => {
        logger.error(str);

        return '';
    });
    process.exit(1);
}

const logger: ILogger = new ConsoleLogger();

program.Command.prototype.unknownOption = (flag: string): void => {
    logger.error(`Unknown arguments: ${flag}`);
    showErrorHelp();
};

program
    .name('botskills disconnect')
    .description('Disconnect a specific skill from your assitant bot')
    .option('-n, --skillName <name>', 'Name or id of the skill to remove from your assistant')
    .option('-f, --skillsFile <path>', 'Path to the assistant Skills configuration file')
    .option('--cs', 'Determine your assistant project structure to be a CSharp-like structure')
    .option('--ts', 'Determine your assistant project structure to be a TypeScript-like structure')
    .option('--verbose', '[OPTIONAL] Output detailed information about the processing of the tool')
    .action((cmd: program.Command, actions: program.Command) => undefined);

const args: program.Command = program.parse(process.argv);

if (process.argv.length < 3) {
    program.help();
}

logger.isVerbose = args.verbose;

// cs and ts validation
if (args.cs && args.ts) {
    logger.error(`Only one of the arguments 'cs' and 'ts' should be provided`);
    process.exit(1);
} else if (!args.cs && existsSync(join(resolve('./'), 'package.json'))) {
    args.ts = true;
} else if (!args.cs && !args.ts) {
    logger.error(`One of the arguments 'cs' or 'ts' should be provided`);
    process.exit(1);
}

// Validation of arguments
// skillsFile validation
if (!args.skillsFile) {
    args.skillsFile = args.ts ? join('src', 'skills.json') : 'skills.json';
} else if (extname(args.skillsFile) !== '.json') {
    logger.error(`The 'skillsFile' argument should be a JSON file.`);
    process.exit(1);
}
const skillsFilePath: string = isAbsolute(args.skillsFile) ? args.skillsFile : join(resolve('./'), args.skillsFile);
logger.message(join(resolve('./'), args.skillsFile));
if (!existsSync(skillsFilePath)) {
    logger.error(
    `The 'skillsFile' argument leads to a non-existing file.
Please make sure to provide a valid path to your Assistant Skills configuration file.`);
    process.exit(1);
}

// skillName validation
if (!args.skillName) {
    logger.error(`The 'skillName' argument should be provided.`);
    process.exit(1);
}
const configuration: IDisconnectConfiguration = {
    skillName: args.skillName,
    skillsFile: skillsFilePath,
    logger: logger
};

disconnectSkill(configuration);
