# MSBuildLoggers

�v���O�������K�����˂�MSBuild�p�̃��K�[�ł��B
Jenkins�Ńr���h�v���W�F�N�g�ւ���DLL��z������ɂ́A
Custom Tools Plugin (https://wiki.jenkins-ci.org/display/JENKINS/Custom+Tools+Plugin) ���g�p����ƊȒP�ł��B

## MinimalSummaryConsoleLogger

���̃��K�[�̓G���[�E�x���̏o�͂ƃT�}���[�̏o�͂��s���܂��B
�e���ڂ��^�u��؂�ŏo�͂��Ă���̂�Excel�փR�s�y���ăt�B���^�ł��܂��B
�܂��A����̃G���R�[�h��UTF-8�Ƃ��Ă��邽�߁AJenkins��MSBuild Pugin���g�p���Ă����{�ꂪ�����������܂���B

### Example for jenkins

MSBuild �^�X�N�̈����Ɏ��̂悤�ɋL�q���܂�:
/noconsolelogger /logger:MinimalSummaryConsoleLogger,%MSBuildLoggers_HOME%\MSBuildLoggers.dll;TrimPath="%WORKSPACE%\\"