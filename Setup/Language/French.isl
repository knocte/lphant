; *** Inno Setup version 4.1.8+ French messages ***
;
; To download user-contributed translations of this file, go to:
;   http://www.jrsoftware.org/is3rdparty.php
;
; Note: When translating this text, do not add periods (.) to the end of
; messages that didn't have them already, because on those messages Inno
; Setup adds the periods automatically (appending a period would result in
; two periods being displayed).
;
; $jrsoftware: issrc/Files/Default.isl,v 1.42 2004/01/22 00:49:23 jr Exp $
; Pierre Yager                                2004/01/29
; Frédéric Bonduelle                          2004/01/30
; Pierre Yager                                2004/02/09
; Pierre Yager                                2004/02/26

[LangOptions]
LanguageName=Français
LanguageID=$040C
; Si la langue dans laquelle vous traduisez InnoSetup requiert des polices ou des
; tailles différentes des valeurs par défaut, dé-commentez les lignes suivantes 
; nécessaires et modifiez les en conséquence.
;DialogFontName=
;DialogFontSize=8
;WelcomeFontName=Verdana
;WelcomeFontSize=12
;TitleFontName=Arial
;TitleFontSize=29
;CopyrightFontName=Arial
;CopyrightFontSize=8

[Messages]

; *** Application titles
SetupAppTitle=Installation
SetupWindowTitle=Installation de %1
UninstallAppTitle=Désinstallation
UninstallAppFullTitle=Désinstallation de %1

; *** Misc. common
InformationTitle=Information
ConfirmTitle=Confirmation
ErrorTitle=Erreur

; *** SetupLdr messages
SetupLdrStartupMessage=Cet assistant va installer %1. Voulez-vous poursuivre ?
LdrCannotCreateTemp=Impossible de créer un fichier temporaire. Abandon de l'installation
LdrCannotExecTemp=Impossible d'exécuter un fichier depuis le dossier temporaire. Abandon de l'installation

; *** Startup error messages
LastErrorMessage=%1.%n%nErreur %2 : %3
SetupFileMissing=Le fichier %1 est absent du dossier d'installation. Veuillez corriger le problème ou vous procurer une nouvelle copie du logiciel.
SetupFileCorrupt=Les fichiers d'installation sont altérés. Veuillez vous procurer une nouvelle copie du logiciel.
SetupFileCorruptOrWrongVer=Les fichiers d'installation sont altérés ou ne sont pas compatibles avec cette version de l'assistant d'installation. Veuillez corriger le problème ou vous procurer une nouvelle copie du logiciel.
NotOnThisPlatform=Ce logiciel ne fonctionnera pas sous %1.
OnlyOnThisPlatform=Ce logiciel ne peut fonctionner que sous %1.
WinVersionTooLowError=Ce logiciel requiert la version %2 ou supérieure de %1.
WinVersionTooHighError=Ce logiciel ne peut pas être installé sous %1 version %2 ou supérieure.
AdminPrivilegesRequired=Vous devez disposer des droits d'administration de cet ordinateur pour installer ce logiciel.
PowerUserPrivilegesRequired=Vous devez disposer des droits d'administration ou faire partie du groupe "Utilisateurs avec pouvoir" de cet ordinateur pour installer ce logiciel.
SetupAppRunningError=L'assistant d'installation a détecté que %1 est actuellement en cours d'exécution.%n%nVeuillez fermer toutes les instances de cette application puis appuyer sur OK pour continuer, ou bien appuyer sur Annuler pour abandonner l'installation.
UninstallAppRunningError=La procédure de désinstallation a détecté que %1 est actuellement en cours d'exécution.%n%nVeuillez fermer toutes les instances de cette application  puis appuyer sur OK pour continuer, ou bien appuyer sur Annuler pour abandonner la désinstallation.

; *** Misc. errors
ErrorCreatingDir=L'assistant d'installation n'a pas pu créer le dossier "%1"
ErrorTooManyFilesInDir=L'assistant d'installation n'a pas pu créer un fichier dans le dossier "%1" car celui-ci contient trop de fichiers

; *** Setup common messages
ExitSetupTitle=Quitter l'installation
ExitSetupMessage=L'installation n'est pas terminée. Si vous abandonnez maintenant, le logiciel ne sera pas installé.%n%nVous devrez relancer cet assistant pour finir l'installation.%n%nVoulez-vous quand même quitter l'assistant d'installation ?
AboutSetupMenuItem=&A propos...
AboutSetupTitle=A Propos de l'assistant d'installation
AboutSetupMessage=%1 version %2%n%3%n%n%1 page d'accueil :%n%4
AboutSetupNote=

; *** Buttons
ButtonBack=< &Précédent
ButtonNext=&Suivant >
ButtonInstall=&Installer
ButtonOK=OK
ButtonCancel=Annuler
ButtonYes=&Oui
ButtonYesToAll=Oui pour &tout
ButtonNo=&Non
ButtonNoToAll=N&on pour tout
ButtonFinish=&Terminer
ButtonBrowse=&Parcourir...
ButtonWizardBrowse=Pa&rcourir...
ButtonNewFolder=&Nouveau dossier

; *** "Select Language" dialog messages
SelectLanguageTitle=Langue de l'assistant d'installation
SelectLanguageLabel=Veuillez sélectionner la langue qui sera utilisée par l'assistant d'installation :

; *** Common wizard text
ClickNext=Appuyez sur Suivant pour continuer ou sur Annuler pour abandonner l'installation.
BeveledLabel=
BrowseDialogTitle=Choix d'un dossier
BrowseDialogLabel=Veuillez choisir un dossier de destination, puis appuyez sur OK.
NewFolderName=Nouveau dossier

; *** "Welcome" wizard page
WelcomeLabel1=Bienvenue dans l'installation de [name]
WelcomeLabel2=Cet assistant va vous guider pour l'installation de [name/ver] sur votre ordinateur.%n%nIl est recommandé de fermer toutes les applications actives avant de poursuivre.

; *** "Password" wizard page
WizardPassword=Mot de passe
PasswordLabel1=Cette installation est protégée par un mot de passe.
PasswordLabel3=Veuillez saisir votre mot de passe (attention à la distinction entre majuscules et minuscules) puis appuyez sur Suivant pour continuer.
PasswordEditLabel=Mot de &passe :
IncorrectPassword=Le mot de passe saisi n'est pas valide. Essayez à nouveau.

; *** "License Agreement" wizard page
WizardLicense=Accord de la licence d'utilisation
LicenseLabel=Veuillez lire les informations importantes ci-dessous avant de continuer.
LicenseLabel3=Veuillez lire le contrat de la licence d'utilisation ci-dessous. Vous devez accepter tous les termes de ce contrat avant de poursuivre l'installation.
LicenseAccepted=J'&accepte les termes du contrat
LicenseNotAccepted=Je n'accepte &pas les termes du contrat

; *** "Information" wizard pages
WizardInfoBefore=Information
InfoBeforeLabel=Veuillez lire les informations importantes ci-dessous avant de continuer.
InfoBeforeClickLabel=Appuyez sur Suivant lorsque vous serez prêt(e) à poursuivre l'installation.
WizardInfoAfter=Informations
InfoAfterLabel=Veuillez lire les informations importantes ci-dessous avant de continuer.
InfoAfterClickLabel=Appuyez sur Suivant lorsque vous serez prêt(e) à poursuivre l'installation.

; *** "User Information" wizard page
WizardUserInfo=Informations sur l'Utilisateur
UserInfoDesc=Veuillez saisir les informations qui vous concernent.
UserInfoName=&Nom d'utilisateur :
UserInfoOrg=&Société :
UserInfoSerial=Numéro de &série :
UserInfoNameRequired=Vous devez au moins saisir un nom.

; *** "Select Destination Location" wizard page
WizardSelectDir=Dossier de destination
SelectDirDesc=Où [name] doit-il être installé ?
SelectDirLabel3=L'assistant va installer [name] dans le dossier suivant.
SelectDirBrowseLabel=Pour continuer, appuyez sur Suivant. Si vous souhaitez choisir un dossier différent, appuyez sur Parcourir.
DiskSpaceMBLabel=Le logiciel requiert au moins [mb] Mo d'espace disque disponible.
ToUNCPathname=L'assistant ne peut pas procéder à l'installation à un emplacement défini par un chemin UNC. Si vous souhaitez effectuer cette installation sur un réseau, vous devez au préalable connecter un lecteur réseau.
InvalidPath=Vous devez saisir un chemin complet comprenant la lettre identifiant l'unité, par exemple :%n%nC:\DOSSIER%n%nou un chemin UNC de la forme :%n%n\\serveur\partage
InvalidDrive=L'unité ou l'emplacement UNC que vous avez sélectionné(e) n'existe pas ou n'est pas accessible. Veuillez choisir une autre destination.
DiskSpaceWarningTitle=L'espace disque disponible est insuffisant
DiskSpaceWarning=L'assistant a besoin d'au moins %1 Ko d'espace disque disponible pour effectuer l'installation, mais l'unité que vous avez sélectionnée ne dispose que de %2 Ko d'espace disponible.%n%nSouhaitez-vous poursuivre malgré tout ?
DirNameTooLong=Le nom ou le chemin du dossier est trop long.
InvalidDirName=Le nom du dossier est invalide.
BadDirName32=Le nom du dossier ne doit contenir aucun des caractères suivants :%n%n%1
DirExistsTitle=Dossier existant
DirExists=Le dossier :%n%n%1%n%nexiste déjà. Souhaitez-vous l'utiliser quand même ?
DirDoesntExistTitle=Le dossier n'existe pas
DirDoesntExist=Le dossier %n%n%1%n%nn'existe pas. Souhaitez-vous que ce dossier soit créé ?

; *** "Select Components" wizard page
WizardSelectComponents=Composants à installer
SelectComponentsDesc=Quels composants de l'application souhaitez-vous installer ?
SelectComponentsLabel2=Sélectionnez les composants que vous désirez installer; décochez les composants que vous ne désirez pas installer. Appuyez ensuite sur Suivant pour poursuivre l'installation.
FullInstallation=Installation complète
; if possible don't translate 'Compact' as 'Minimal' (I mean 'Minimal' in your language)
CompactInstallation=Installation compacte
CustomInstallation=Installation personnalisée
NoUninstallWarningTitle=Composants existants
NoUninstallWarning=L'assistant d'installation a détecté que les composants suivants sont déjà installés sur votre système :%n%n%1%n%nDécocher ces composants ne les désinstallera pas pour autant.%n%nVoulez-vous tout-de-même continuer ?
ComponentSize1=%1 Ko
ComponentSize2=%1 Mo
ComponentsDiskSpaceMBLabel=Les composants sélectionnés nécessitent au moins [mb] Mo d'espace disque disponible.

; *** "Select Additional Tasks" wizard page
WizardSelectTasks=Tâches supplémentaires
SelectTasksDesc=Quelles sont les tâches supplémentaires qui doivent être effectuées ?
SelectTasksLabel2=Sélectionnez les tâches supplémentaires que l'assistant d'installation doit effectuer pendant l'installation de [name], puis appuyez sur Suivant.

; *** "Select Start Menu Folder" wizard page
WizardSelectProgramGroup=Sélection du dossier du menu Démarrer
SelectStartMenuFolderDesc=Où l'assistant d'installation doit-il placer les raccourcis du logiciel ?
SelectStartMenuFolderLabel3=L'assistant va créer les raccourcis du logiciel dans le dossier du menu Démarrer indiqué ci-dessous.
SelectStartMenuFolderBrowseLabel=Appuyez sur Suivant pour continuer. Appuyez sur Parcourir si vous souhaitez sélectionner un autre dossier du menu Démarrer.
NoIconsCheck=&Ne pas créer d'icône
MustEnterGroupName=Vous devez saisir un nom de dossier du menu Démarrer.
GroupNameTooLong=Le nom ou le chemin du dossier est trop long.
InvalidGroupName=Le nom du dossier n'est pas valide.
BadGroupName=Le nom du dossier ne doit contenir aucun des caractères suivants :%n%n%1
NoProgramGroupCheck2=Ne pas créer de &dossier dans le menu Démarrer

; *** "Ready to Install" wizard page
WizardReady=Prêt à installer
ReadyLabel1=L'assistant dispose à présent de toutes les informations pour installer [name] sur votre ordinateur.
ReadyLabel2a=Appuyez sur Installer pour procéder à l'installation ou sur Précédent pour revoir ou modifier une option d'installation.
ReadyLabel2b=Appuyez sur Installer pour procéder à l'installation.
ReadyMemoUserInfo=Informations sur l'Utilisateur :
ReadyMemoDir=Dossier de destination :
ReadyMemoType=Type d'installation :
ReadyMemoComponents=Composants sélectionnés :
ReadyMemoGroup=Dossier du menu Démarrer :
ReadyMemoTasks=Tâches supplémentaires :

; *** "Preparing to Install" wizard page
WizardPreparing=Préparation de l'installation
PreparingDesc=L'assistant d'installation prépare l'installation de [name] sur votre ordinateur.
PreviousInstallNotCompleted=L'installation ou la suppression d'un logiciel précédent n'est pas totalement achevée. Veuillez redémarrer votre ordinateur pour achever cette installation ou suppression.%n%nUne fois votre ordinateur redémarré, veuillez relancer cet assistant pour reprendre l'installation de [name].
CannotContinue=L'assistant ne peut pas continuer. Veuillez appuyer sur Annuler pour abandonner l'installation.

; *** "Installing" wizard page
WizardInstalling=Installation en cours
InstallingLabel=Veuillez patienter pendant que l'assistant installe [name] sur votre ordinateur.

; *** "Setup Completed" wizard page
FinishedHeadingLabel=Fin de l'installation de [name]
FinishedLabelNoIcons=L'assistant a terminé l'installation de [name] sur votre ordinateur.
FinishedLabel=L'assistant a terminé l'installation de [name] sur votre ordinateur. L'application peut être lancée à l'aide des icônes créées sur le Bureau par l'installation.
ClickFinish=Veuillez appuyer sur Terminer pour quitter l'assistant d'installation.
FinishedRestartLabel=L'assistant doit redémarrer votre ordinateur pour terminer l'installation de [name].%n%nVoulez-vous redémarrer maintenant ?
FinishedRestartMessage=L'assistant doit redémarrer votre ordinateur pour terminer l'installation de [name].%n%nVoulez-vous redémarrer maintenant ?
ShowReadmeCheck=Oui, je souhaite lire le fichier LisezMoi
YesRadio=&Oui, redémarrer l'ordinateur maintenant
NoRadio=&Non, je préfère redémarrer l'ordinateur plus tard
; used for example as 'Run MyProg.exe'
RunEntryExec=Exécuter %1
; used for example as 'View Readme.txt'
RunEntryShellExec=Voir %1

; *** "Setup Needs the Next Disk" stuff
ChangeDiskTitle=L'assistant a besoin du disque suivant
SelectDiskLabel2=Veuillez insérer le disque %1 et appuyer sur OK.%n%nSi les fichiers de ce disque se trouvent à un emplacement différent de celui indiqué ci-dessous, veuillez saisir le chemin correspondant ou appuyez sur Parcourir.
PathLabel=&Chemin :
FileNotInDir2=Le fichier "%1" ne peut pas être trouvé dans "%2". Veuillez insérer le disque correct ou sélectionner un autre dossier.
SelectDirectoryLabel=Veuillez indiquer l'emplacement du disque suivant.

; *** Installation phase messages
SetupAborted=L'installation n'est pas terminée.%n%nVeuillez corriger le problème et relancer l'installation.
EntryAbortRetryIgnore=Appuyez sur Reprendre pour essayer à nouveau, sur Ignorer pour continuer quand même, ou sur Annuler pour abandonner l'installation.

; *** Installation status messages
StatusCreateDirs=Création des dossiers...
StatusExtractFiles=Extraction des fichiers...
StatusCreateIcons=Création des raccourcis...
StatusCreateIniEntries=Création des entrées du fichier INI...
StatusCreateRegistryEntries=Création des entrées de registre...
StatusRegisterFiles=Enregistrement des fichiers...
StatusSavingUninstall=Sauvegarde des informations de désinstallation...
StatusRunProgram=Finalisation de l'installation...
StatusRollback=Annulation des modifications...

; *** Misc. errors
ErrorInternal2=Erreur interne : %1
ErrorFunctionFailedNoCode=%1 a échoué
ErrorFunctionFailed=%1 a échoué; code %2
ErrorFunctionFailedWithMessage=%1 a échoué; code %2.%n%3
ErrorExecutingProgram=Impossible d'exécuter le fichier :%n%1

; *** Registry errors
ErrorRegOpenKey=Erreur lors de l'ouverture de la clé de registre :%n%1\%2
ErrorRegCreateKey=Erreur lors de la création de la clé de registre :%n%1\%2
ErrorRegWriteKey=Erreur lors de l'écriture de la clé de registre :%n%1\%2

; *** INI errors
ErrorIniEntry=Erreur d'écriture d'une entrée dans le fichier INI "%1" .

; *** File copying errors
FileAbortRetryIgnore=Appuyez sur Reprendre pour essayer à nouveau, sur Ignorer pour passer outre ce fichier (non recommandé), ou sur Annuler pour abandonner l'installation.
FileAbortRetryIgnore2=Appuyez sur Reprendre pour essayer à nouveau, sur Ignorer pour poursuivre malgré tout (non recommandé), ou sur Annuler pour abandonner l'installation.
SourceIsCorrupted=Le fichier source est altéré
SourceDoesntExist=Le fichier source "%1" n'existe pas
ExistingFileReadOnly=Le fichier existant est marqué en lecture seule.%n%nAppuyez sur Reprendre pour retirer l'attribut lecture seule et recommencer, sur Ignorer pour passer outre ce fichier, ou sur Annuler pour abandonner l'installation.
ErrorReadingExistingDest=Une erreur s'est produite lors de la tentative de lecture du fichier existant :
FileExists=Le fichier existe déjà.%n%nSouhaitez-vous que l'installation le remplace ?
ExistingFileNewer=Le fichier existant est plus récent que celui que l'assistant essaie d'installer. Il est recommandé de conserver le fichier existant.%n%nSouhaitez-vous conserver le fichier existant ?
ErrorChangingAttr=Une erreur est survenue en essayant de modifier les attributs du fichier existant :
ErrorCreatingTemp=Une erreur est survenue en essayant de créer un fichier dans le dossier de destination :
ErrorReadingSource=Une erreur est survenue lors de la lecture du fichier source :
ErrorCopying=Une erreur est survenue lors de la copie d'un fichier :
ErrorReplacingExistingFile=Une erreur est survenue lors du remplacement d'un fichier existant :
ErrorRestartReplace=Le marquage d'un fichier pour remplacement au redémarrage de l'ordinateur a échoué:
ErrorRenamingTemp=Une erreur est survenue en essayant de renommer un fichier dans le dossier de destination :
ErrorRegisterServer=Impossible d'enregistrer la DLL ou l'OCX : %1
ErrorRegisterServerMissingExport=La fonction exportée DllRegisterServer n'a pas été trouvée
ErrorRegisterTypeLib=Impossible d'enregistrer la librairie de types : %1

; *** Post-installation errors
ErrorOpeningReadme=Une erreur est survenue à l'ouverture du fichier LisezMoi.
ErrorRestartingComputer=L'installation n'a pas pu redémarrer l'ordinateur. Merci de bien vouloir le faire vous-même.

; *** Uninstaller messages
UninstallNotFound=Le fichier "%1" n'existe pas. Impossible de désinstaller.
UninstallOpenError=Le fichier "%1" n'a pas pu être ouvert. Impossible de désinstaller
UninstallUnsupportedVer=Le format du fichier journal de désinstallation "%1" n'est pas reconnu par cette version de la procédure de désinstallation. Impossible de désinstaller
UninstallUnknownEntry=Une entrée inconnue (%1) a été rencontrée dans le fichier journal de désinstallation
ConfirmUninstall=Voulez-vous vraiment désinstaller complètement %1 ainsi que tous ses composants ?
OnlyAdminCanUninstall=Ce logiciel ne peut être désinstallé que par un utilisateur disposant des droits d'administration.
UninstallStatusLabel=Veuillez patienter pendant que %1 est retiré de votre ordinateur.
UninstalledAll=%1 a été correctement désinstallé de cet ordinateur.
UninstalledMost=La désinstallation de %1 est terminée.%n%nCertains éléments n'ont pu être supprimés automatiquement. Vous pouvez les supprimer manuellement.
UninstalledAndNeedsRestart=Vous devez redémarrer l'ordinateur pour terminer la désinstallation de %1.%n%nVoulez-vous redémarrer maintenant ?
UninstallDataCorrupted=Le ficher "%1" est altéré. Impossible de désinstaller

; *** Uninstallation phase messages
ConfirmDeleteSharedFileTitle=Supprimer les fichiers partagés ?
ConfirmDeleteSharedFile2=Le système indique que les fichiers suivants sont partagés mais ne sont plus utilisés par d'autres applications. Souhaitez-vous que la procédure de désinstallation supprime ces fichiers partagés ?%n%nSi certaines applications ont besoin de ces fichiers et qu'il sont supprimés, ces applications risquent de ne plus fonctionner correctement. En cas de doute, appuyez sur Non; Le fait de laisser ce fichier sur votre système ne causera aucun dommage.
SharedFileNameLabel=Nom du fichier :
SharedFileLocationLabel=Emplacement :
WizardUninstalling=Etat de la désinstallation
StatusUninstalling=Désinstallation de %1...