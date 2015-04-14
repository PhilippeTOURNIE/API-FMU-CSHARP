/* ---------------------------------------------------------------------------*
* Modele FMU
* @CSTB Philippe TOURNIE
* ---------------------------------------------------------------------------*/
// Aide compiler en c :
//http://stackoverflow.com/questions/8579894/compile-c-app-with-visual-studio-2012

//////////////////////////////////////////////////////////////////////////////////////
//Partie à configurer


// Nom du modele ici pour le test c'est value
// en DLL Share il ne faut pas le définir #define FMI2_FUNCTION_PREFIX MyModel_ values
#define MODEL_IDENTIFIER values
// Guid xml et dll doivent correspondre
#define MODEL_GUID "{8c4e810f-3df3-4a00-8276-176fa3c9f004}"

//////////////////////////////////////////////////////////////////////////////////////
/*
FMI2 Consigne ->
The FMU must be distributed :
DLLs on Windows machines must be built with option “MT” to include the run-time environment
of VisualStudio in the DLL, and not use the option “MD” where this is not the case].*/

// include fmu header files, typedefs and macros
#include "atlbase.h"
#include "atlsafe.h"        // CComSafeArray   
#include "fmi2Functions.h"
#import "Lib\\FMU2ManageCsharp.tlb"  raw_interfaces_only



//***************************************************
// Configuration du modèle FMU
//***************************************************


FMU2ManageCsharp::_FMU2Manage* pFMUManage;


_variant_t _VariantComponent; //TODO Attention cette variable doit disparaitre elle 
// empeche d'avoir plusieur FMU de même type

FMU2ManageCsharp::_FMU2Manage* LinkAPI()
{
	FMU2ManageCsharp::_FMU2Manage *_pFMUManage;
	//{F1158F17-A005-1605-4AE2-8C7039F2D445}	
	HRESULT hr = CoInitialize(NULL);
	CLSID clsidM;
	clsidM.Data1 = 4044721943;
	clsidM.Data2 = 40965;
	clsidM.Data3 = 5637;
	clsidM.Data4[0] = 74;
	clsidM.Data4[1] = 226;
	clsidM.Data4[2] = 140;
	clsidM.Data4[3] = 112;
	clsidM.Data4[4] = 57;
	clsidM.Data4[5] = 242;
	clsidM.Data4[6] = 212;
	clsidM.Data4[7] = 69;
	hr = CoCreateInstance(clsidM, NULL, CLSCTX_INPROC_SERVER, __uuidof(FMU2ManageCsharp::_FMU2Manage), (LPVOID *)&_pFMUManage);
	return _pFMUManage;
}




//***************************************************
// GESTION DES LOG FMU
//***************************************************
// categories of logging supported by model.
// Value is the index in logCategories of a ModelInstance.
#define LOG_ALL       0
#define LOG_ERROR     1
#define LOG_FMI_CALL  2
#define LOG_EVENT     3

#define NUMBER_OF_CATEGORIES 4


const fmi2CallbackFunctions* _functions;
fmi2String _instanciateName;



/*******************************************************
TOOLS
*********************************************************/
//int VariantBoolToBool(VARIANT_BOOL varFlag)
//{
//	bool boolFlag;
//
//	switch (varFlag)
//	{
//	case VARIANT_TRUE:
//		boolFlag = 1;
//		break;
//	case VARIANT_FALSE:
//		boolFlag = 0;
//		break;
//	default:
//		boolFlag = 0;
//	}
//
//	return boolFlag;
//}
//VARIANT_BOOL BoolToVariantBool(int varFlag)
//{
//	VARIANT_BOOL boolFlag;
//
//	switch (varFlag)
//	{
//	case 1:
//		boolFlag = VARIANT_TRUE;
//		break;
//	case 0:
//		boolFlag = VARIANT_FALSE;
//		break;
//	default:
//		boolFlag = VARIANT_FALSE;
//	}
//	return boolFlag;
//}

long fmi2StatusKindTolong(fmi2StatusKind s)
{
	switch (s)
	{
	case fmi2DoStepStatus:
		return 0;
	case fmi2PendingStatus:
		return 1;
	case fmi2LastSuccessfulTime:
		return 2;
	case fmi2Terminated:
		return 3;
	default:
		return 3;
	}
} 
fmi2StatusKind longTofmi2StatusKind(long s)
{
	switch (s)
	{
	case 0:
		return fmi2DoStepStatus;
	case 1:
		return fmi2PendingStatus;
	case 2:
		return fmi2LastSuccessfulTime;
	case 3:
		return fmi2Terminated;
	default:
		return fmi2Terminated;
	}
}

fmi2Status longTofmi2Status(long aStatus)
{

	switch (aStatus)
	{
	case 0:
		return fmi2OK;
	case 1:
		return fmi2Warning;
	case 2:
		return fmi2Discard;
	case 3:
		return	fmi2Error;
	case 4:
		return	fmi2Fatal;
	case 5:
		return	fmi2Pending;
	default:
		return fmi2Fatal;
	}
}


SAFEARRAY* ArrayDoubleToSAFEARRAY(CComSafeArray<DOUBLE> valeur, double value[], size_t nvr)
{
	for (int i = 0; i < nvr; i++)
		valeur[i] = value[i];
	return valeur.Detach();
}
SAFEARRAY* ArrayDoubleToSAFEARRAY(CComSafeArray<DOUBLE> valeur, const double value[], size_t nvr)
{
	for (int i = 0; i < nvr; i++)
		valeur[i] = value[i];
	return valeur.Detach();
}
SAFEARRAY* ArrayIntToSAFEARRAY(CComSafeArray<long> valeur, const unsigned int value[], size_t nvr)
{
	for (int i = 0; i < nvr; i++)
		valeur[i] = value[i];
	return valeur.Detach();
}
SAFEARRAY* ArrayIntToSAFEARRAY(CComSafeArray<long> valeur, int value[], size_t nvr)
{
	for (int i = 0; i < nvr; i++)
		valeur[i] = value[i];
	return valeur.Detach();
}
SAFEARRAY* ArrayIntToSAFEARRAY(CComSafeArray<long> valeur, const int value[], size_t nvr)
{
	for (int i = 0; i < nvr; i++)
		valeur[i] = value[i];
	return valeur.Detach();
}
//SAFEARRAY* ArrayBoolToSAFEARRAY(CComSafeArray<VARIANT_BOOL> valeur, int value[], size_t nvr)
//{
//	for (int i = 0; i < nvr; i++)
//		valeur[i] = BoolToVariantBool(value[i]);
//	return valeur.Detach();
//}
//SAFEARRAY* ArrayBoolToSAFEARRAY(CComSafeArray<VARIANT_BOOL> valeur, const int value[], size_t nvr)
//{
//	for (int i = 0; i < nvr; i++)
//		valeur[i] = BoolToVariantBool(value[i]);
//	return valeur.Detach();
//}
//SAFEARRAY* ArrayStringToSAFEARRAY(CComSafeArray<_bstr_t> valeur, fmi2String value[], size_t nvr){
//	for (int i = 0; i < nvr; i++)
//		valeur[i] = value[i];
//	return valeur.Detach();
//}

//inverse
void SAFEARRAYToArrayString(CComSafeArray<BSTR> valeur, fmi2String value[], size_t nvr)
{
	for (int i = 0; i < nvr; i++)
	{
		_bstr_t bstrStart(valeur[i]);
		value[i] = _strdup(bstrStart);
			//value[i] = bstrStart;
	}
}
//void SAFEARRAYToArrayBool(CComSafeArray<VARIANT_BOOL> valeur, int value[], size_t nvr)
//{
//	for (int i = 0; i < nvr; i++)
//	{
//		value[i] = VariantBoolToBool(valeur[i]);
//	}
//}
void SAFEARRAYToArrayInt(CComSafeArray<long> valeur, int value[], size_t nvr)
{
	for (int i = 0; i < nvr; i++)
		value[i] = valeur[i];
}
void SAFEARRAYToArrayDouble(CComSafeArray<double> valeur, double value[], size_t nvr)
{
	for (int i = 0; i < nvr; i++)
		value[i] = valeur[i];
}
/*******************************************************
 FIN REGION TOOLS
********************************************************/

/***************************************************
Common Functions
****************************************************/

/* Inquire version numbers of header files and setting logging status */
const char* fmi2GetTypesPlatform(void){ return fmi2TypesPlatform; }
const char* fmi2GetVersion(void){ return fmi2Version; }
fmi2Status fmi2SetDebugLogging(fmi2Component c, fmi2Boolean loggingOn, size_t nCategories, const fmi2String categories[])
{

	// ignore arguments: nCategories, categories	

	if ((pFMUManage) == 0) // pointeur null
		return fmi2Error;

	return fmi2OK;

}

/* Enter and exit initialization mode, terminate and reset */
fmi2Component fmi2Instantiate(fmi2String instanciateName,
	fmi2Type fmuType,
	fmi2String fmuGUID,
	fmi2String fmuResourceLocation,
	const fmi2CallbackFunctions* functions,
	fmi2Boolean visible,
	fmi2Boolean loggingOn)
{
	_instanciateName = instanciateName;
	_functions = functions;

	if (!functions->allocateMemory || !functions->freeMemory) {
		functions->logger(functions->componentEnvironment, instanciateName, fmi2Error, "error",
			"fmi2Instantiate: Missing callback function.");
		return NULL;
	}
	if (!instanciateName || strlen(instanciateName) == 0) {
		functions->logger(functions->componentEnvironment, "?", fmi2Error, "error",
			"fmi2Instantiate: Missing instance name.");
		return NULL;
	}
	if (!fmuGUID || strlen(fmuGUID) == 0) {
		functions->logger(functions->componentEnvironment, instanciateName, fmi2Error, "error",
			"fmi2Instantiate: Missing GUID.");
		return NULL;
	}
	if (strcmp(fmuGUID, MODEL_GUID)) {
		functions->logger(functions->componentEnvironment, instanciateName, fmi2Error, "error",
			"fmi2Instantiate: Wrong GUID %s. Expected %s.", fmuGUID, MODEL_GUID);
		return NULL;
	}

	//Pas besoin de gérer la mémoire ici c'est C# qui le fait...

	//{F1158F17-A005-1605-4AE2-8C7039F2D445}	
	HRESULT hr = CoInitialize(NULL);
	CLSID clsidM;
	clsidM.Data1 = 4044721943;
	clsidM.Data2 = 40965;
	clsidM.Data3 = 5637;
	clsidM.Data4[0] = 74;
	clsidM.Data4[1] = 226;
	clsidM.Data4[2] = 140;
	clsidM.Data4[3] = 112;
	clsidM.Data4[4] = 57;
	clsidM.Data4[5] = 242;
	clsidM.Data4[6] = 212;
	clsidM.Data4[7] = 69;
	hr = CoCreateInstance(clsidM, NULL, CLSCTX_INPROC_SERVER, __uuidof(FMU2ManageCsharp::_FMU2Manage), (LPVOID *)&pFMUManage);

	if (FAILED(hr))
	{
		//Put message failed instanced log
		functions->logger(functions->componentEnvironment, instanciateName, fmi2Error, "error",
			"fmi2Instantiate: Failed with API");
	}

	// Affectation des variable à l'API	
	_bstr_t bstr1 = instanciateName;
	_bstr_t bstr2 = fmuGUID;
	_bstr_t bstr3 = fmuResourceLocation;
	long l = fmuType;
	
	HRESULT hres = pFMUManage->Instantiate(bstr1, l, bstr2, bstr3, visible, loggingOn, &_VariantComponent);
	void* pComponent = NULL;	

	
	pComponent = (void*)&_VariantComponent;
	
	//Log sont tous à "on" par defaut pour le moment
		
	printf("ok Instance \n");
	return pComponent;

}
void fmi2FreeInstance(fmi2Component c)
{

	if (!c) return;
	//if (invalidState(comp, "fmi2FreeInstance", MASK_fmi2FreeInstance))
	//	return;
	
	//pas de gestion de mémoire... juste des pointeur en safe côté C#
	_variant_t myVariantComponent = (_variant_t*)c;
	pFMUManage->FreeInstance(myVariantComponent);


}
fmi2Status fmi2SetupExperiment(fmi2Component c, fmi2Boolean toleranceDefined, fmi2Real tolerance, fmi2Real startTime, fmi2Boolean stopTimeDefined, fmi2Real stopTime)
{	
	try{
		
		_variant_t myVariantComponent = (_variant_t*)c;				
		long res;		
		long btoleranceDefined = toleranceDefined;
		long bstopTimeDefined=stopTimeDefined;
		double dtolerance = tolerance;
		double dstartTime = startTime;
		double dstopTime = stopTime;				
		
		//_pFMUManage=LinkAPI();
		pFMUManage->SetupExperiment(myVariantComponent,
			btoleranceDefined,
			dtolerance, dstartTime, bstopTimeDefined, dstopTime, &res);		
		
		return longTofmi2Status(res);
	}
	catch (int e){
		printf("erreur: %d: \n",e);
		_functions->logger(_functions->componentEnvironment, _instanciateName, fmi2Error, "error",
			"fmi2SetupExperiment: %d", e);
		return fmi2Error;
	}
}
fmi2Status fmi2EnterInitializationMode(fmi2Component c)
{
	try
	{
		_variant_t myVariantComponent = (_variant_t*)c;
		long res;
		pFMUManage->EnterInitializationMode(myVariantComponent, &res);
		return longTofmi2Status(res);
	}
	catch (int e){
		_functions->logger(_functions->componentEnvironment, _instanciateName, fmi2Error, "error",
			"fmi2EnterInitializationMode: %d", e);
		return fmi2Error;
	}
}
fmi2Status fmi2ExitInitializationMode(fmi2Component c){
	try{
		_variant_t myVariantComponent = (_variant_t*)c;
		long res;
		pFMUManage->ExitInitializationMode(myVariantComponent, &res);
		return longTofmi2Status(res);
	}
	catch (int e){
		_functions->logger(_functions->componentEnvironment, _instanciateName, fmi2Error, "error",
			"fmi2ExitInitializationMode: %d", e);
		return fmi2Error;
	}
}
fmi2Status fmi2Terminate(fmi2Component c){
	try{
		_variant_t myVariantComponent = (_variant_t*)c;
		long res;
		pFMUManage->Terminate(myVariantComponent, &res);
		return longTofmi2Status(res);
	}
	catch (int e){
		_functions->logger(_functions->componentEnvironment, _instanciateName, fmi2Error, "error",
			"fmi2Terminate: %d", e);
		return fmi2Error;
	}
}
fmi2Status fmi2Reset(fmi2Component c)
{
	try{
		_variant_t myVariantComponent = (_variant_t*)c;
		long res;
		pFMUManage->Terminate(myVariantComponent, &res);
		return longTofmi2Status(res);
	}
	catch (int e){
		_functions->logger(_functions->componentEnvironment, _instanciateName, fmi2Error, "error",
			"fmi2Reset: %d", e);
		return fmi2Error;
	}
}

/* Getting and setting variable values */
fmi2Status fmi2GetReal(fmi2Component c, const fmi2ValueReference vr[], size_t nvr, fmi2Real value[]){
	try{
		_variant_t myVariantComponent = (_variant_t*)c;
		long res;
		// ref
		CComSafeArray<long> ValeursRef(nvr);
		SAFEARRAY* ValeursRefSafe = ArrayIntToSAFEARRAY(ValeursRef, vr, nvr);
		// Value Double
		CComSafeArray<DOUBLE> ValeursData(nvr);
		SAFEARRAY* ValeursDataSafe = ArrayDoubleToSAFEARRAY(ValeursData, value, nvr);
		// Appel
		pFMUManage->GetReal(myVariantComponent, ValeursRefSafe, nvr, &ValeursDataSafe, &res);
		//affecte les résultats
		ValeursData.Attach(ValeursDataSafe);

		SAFEARRAYToArrayDouble(ValeursData, value, nvr);

		return longTofmi2Status(res);
	}
	catch (int e){
		_functions->logger(_functions->componentEnvironment, _instanciateName, fmi2Error, "error",
			"fmi2GetReal: %d", e);
		return fmi2Error;
	}
}
fmi2Status fmi2GetInteger(fmi2Component c, const fmi2ValueReference vr[], size_t nvr, fmi2Integer value[])
{
	try{
		_variant_t myVariantComponent = (_variant_t*)c;
		long res;
		// ref
		CComSafeArray<long> ValeursRef(nvr);
		SAFEARRAY* ValeursRefSafe = ArrayIntToSAFEARRAY(ValeursRef, vr, nvr);
		// Value to pass
		CComSafeArray<long> ValeursData(nvr);
		SAFEARRAY* ValeursDataSafe = ArrayIntToSAFEARRAY(ValeursData, value, nvr);
		// Appel
		pFMUManage->GetInteger(myVariantComponent, ValeursRefSafe, nvr, &ValeursDataSafe, &res);
		//affecte les résultats
		if (ValeursDataSafe == NULL)
		{
			_functions->logger(_functions->componentEnvironment, _instanciateName, fmi2Error, "error",
				"fmi2GetInteger: Null pointer");
			return fmi2Error;
		}
		ValeursData.Attach(ValeursDataSafe);

		SAFEARRAYToArrayInt(ValeursData, value, nvr);

		return longTofmi2Status(res);
	}
	catch (int e){
		_functions->logger(_functions->componentEnvironment, _instanciateName, fmi2Error, "error",
			"fmi2GetInteger: %d", e);
		return fmi2Error;
	}
}
fmi2Status fmi2GetBoolean(fmi2Component c, const fmi2ValueReference vr[], size_t nvr, fmi2Boolean value[]){
	try{
		_variant_t myVariantComponent = (_variant_t*)c;
		long res;
		// ref
		CComSafeArray<long> ValeursRef(nvr);
		SAFEARRAY* ValeursRefSafe = ArrayIntToSAFEARRAY(ValeursRef, vr, nvr);
		// Value to pass
		CComSafeArray<long> ValeursData(nvr);
		SAFEARRAY* ValeursDataSafe = ArrayIntToSAFEARRAY(ValeursData, value, nvr);
		// Appel
		pFMUManage->GetBoolean(myVariantComponent, ValeursRefSafe, nvr, &ValeursDataSafe, &res);
		//affecte les résultats
		if (ValeursDataSafe == NULL)
		{
			_functions->logger(_functions->componentEnvironment, _instanciateName, fmi2Error, "error",
				"fmi2GetBoolean: Null pointer");
			return fmi2Error;
		}
		ValeursData.Attach(ValeursDataSafe);
		SAFEARRAYToArrayInt(ValeursData, value, nvr);

		return longTofmi2Status(res);
	}
	catch (int e){
		_functions->logger(_functions->componentEnvironment, _instanciateName, fmi2Error, "error",
			"fmi2GetBoolean: %d", e);
		return fmi2Error;
	}
}
fmi2Status fmi2GetString(fmi2Component c, const fmi2ValueReference vr[], size_t nvr, fmi2String value[])
{
	
	try{
		_variant_t myVariantComponent = (_variant_t*)c;
		long res;		 
		CComSafeArray<long> ValeursRef(nvr);
		SAFEARRAY* ValeursRefSafe = ArrayIntToSAFEARRAY(ValeursRef, vr, nvr);
		// Value to pass
		CComSafeArray<BSTR> ValeursData(nvr);
		SAFEARRAY* ValeursDataSafe = ValeursData.Detach();
		// Appel
		pFMUManage->GetString(myVariantComponent, ValeursRefSafe, nvr, &ValeursDataSafe, &res);
		//affecte les résultats
		if (ValeursDataSafe == NULL)
		{
			_functions->logger(_functions->componentEnvironment, _instanciateName, fmi2Error, "error",
				"fmi2GetString: Null pointer");
			return fmi2Error;
		}
		ValeursData.Attach(ValeursDataSafe);		

		
		SAFEARRAYToArrayString(ValeursData, value, nvr);

		
		

		return longTofmi2Status(res);
	}
	catch (int e){
		_functions->logger(_functions->componentEnvironment, _instanciateName, fmi2Error, "error",
			"fmi2GetString: %d", e);
		return fmi2Error;
	}
}
fmi2Status fmi2SetReal(fmi2Component c, const fmi2ValueReference vr[], size_t nvr, const fmi2Real value[])
{
	try{
		_variant_t myVariantComponent = (_variant_t*)c;
		long res;
		// ref
		CComSafeArray<long> ValeursRef(nvr);
		SAFEARRAY* ValeursRefSafe = ArrayIntToSAFEARRAY(ValeursRef, vr, nvr);
		// Value Double
		CComSafeArray<DOUBLE> ValeursData(nvr);
		SAFEARRAY* ValeursDataSafe = ArrayDoubleToSAFEARRAY(ValeursData, value, nvr);
		// Appel
		pFMUManage->SetReal(myVariantComponent, ValeursRefSafe, nvr, ValeursDataSafe, &res);

		return longTofmi2Status(res);
	}
	catch (int e){
		_functions->logger(_functions->componentEnvironment, _instanciateName, fmi2Error, "error",
			"fmi2SetReal: %d", e);
		return fmi2Error;
	}
}
fmi2Status fmi2SetInteger(fmi2Component c, const fmi2ValueReference vr[], size_t nvr, const fmi2Integer value[])
{
	try{
		_variant_t myVariantComponent = (_variant_t*)c;
		long res;
		// ref
		CComSafeArray<long> ValeursRef(nvr);
		SAFEARRAY* ValeursRefSafe = ArrayIntToSAFEARRAY(ValeursRef, vr, nvr);
		// Value to pass
		CComSafeArray<long> ValeursData(nvr);
		SAFEARRAY* ValeursDataSafe = ArrayIntToSAFEARRAY(ValeursData, value, nvr);
		// Appel
		pFMUManage->SetInteger(myVariantComponent, ValeursRefSafe, nvr, ValeursDataSafe, &res);

		return longTofmi2Status(res);
	}
	catch (int e){
		_functions->logger(_functions->componentEnvironment, _instanciateName, fmi2Error, "error",
			"fmi2SetInteger: %d", e);
		return fmi2Error;
	}
}
fmi2Status fmi2SetBoolean(fmi2Component c, const fmi2ValueReference vr[], size_t nvr, const fmi2Boolean value[])
{
	try{
		_variant_t myVariantComponent = (_variant_t*)c;
		long res;
		// ref
		CComSafeArray<long> ValeursRef(nvr);
		SAFEARRAY* ValeursRefSafe = ArrayIntToSAFEARRAY(ValeursRef, vr, nvr);
		// Value to pass
		CComSafeArray<long> ValeursData(nvr);
		SAFEARRAY* ValeursDataSafe = ArrayIntToSAFEARRAY(ValeursData, value, nvr);
		// Appel
		pFMUManage->SetBoolean(myVariantComponent, ValeursRefSafe, nvr, ValeursDataSafe, &res);

		return longTofmi2Status(res);
	}
	catch (int e){
		_functions->logger(_functions->componentEnvironment, _instanciateName, fmi2Error, "error",
			"fmi2SetBoolean: %d", e);
		return fmi2Error;
	}
}
fmi2Status fmi2SetString(fmi2Component c, const fmi2ValueReference vr[], size_t nvr, const fmi2String value[])
{
	return fmi2Error;
}


/* Getting and setting the internal FMU state */
fmi2Status fmi2GetFMUstate(fmi2Component c, fmi2FMUstate* FMUstate)
{
	//try{
	//	_variant_t myVariantComponent = (_variant_t*)c;
	//	long res;
	//	pFMUManage->GetFMUstate(myVariantComponent,&res);
	//	return longTofmi2Status(res);
	//}
	//catch (int e){
		_functions->logger(_functions->componentEnvironment, _instanciateName, fmi2Error, "error",
			"fmi2GetFMUstate: Unsupported");
		return fmi2Error;
	//}
	
}
fmi2Status fmi2SetFMUstate(fmi2Component c, fmi2FMUstate FMUstate){ 
	_functions->logger(_functions->componentEnvironment, _instanciateName, fmi2Error, "error",
		"fmi2SetFMUstate: Unsupported");
	return fmi2Error;
}
fmi2Status fmi2FreeFMUstate(fmi2Component c, fmi2FMUstate* FMUstate){
	_functions->logger(_functions->componentEnvironment, _instanciateName, fmi2Error, "error",
		"fmi2FreeFMUstate: Unsupported");
	return fmi2Error;
}
fmi2Status fmi2SerializedFMUstateSize(fmi2Component c, fmi2FMUstate FMUstate, size_t *size){
	_functions->logger(_functions->componentEnvironment, _instanciateName, fmi2Error, "error",
		"fmi2SerializedFMUstateSize: Unsupported");
	return fmi2Error;
}
fmi2Status fmi2SerializeFMUstate(fmi2Component c, fmi2FMUstate FMUstate, fmi2Byte serializeState[], size_t size){
	_functions->logger(_functions->componentEnvironment, _instanciateName, fmi2Error, "error",
		"fmi2SerializeFMUstate: Unsupported");
	return fmi2Error;
}
fmi2Status fmi2DeSerializeFMUstate(fmi2Component c, const fmi2Byte serializeState[], size_t size, fmi2FMUstate* FMUstate){
	_functions->logger(_functions->componentEnvironment, _instanciateName, fmi2Error, "error",
		"fmi2DeSerializeFMUstate:");
	return fmi2Error;
}

/* Getting partial derivatives */
fmi2Status fmi2GetDirectionalDerivative(fmi2Component c, const fmi2ValueReference vrUnknown_ref[], size_t nUnknown,
	const fmi2ValueReference vKnown[], size_t nKnown,
	const fmi2Real dvKnown[], fmi2Real dvUnknown[]){
	_functions->logger(_functions->componentEnvironment, _instanciateName, fmi2Error, "error",
		"fmi2GetDirectionalDerivative: Unsupported");
	return fmi2Error;
}

/***************************************************
// Types for Functions for FMI2 for Model Exchange
****************************************************/

/* Enter and exit the different modes */
fmi2Status fmi2EnterEventMode(fmi2Component c){
	try{
		_variant_t myVariantComponent = (_variant_t*)c;
		long res;
		pFMUManage->EnterEventMode(myVariantComponent, &res);
		return longTofmi2Status(res);
	}
	catch (int e){
		_functions->logger(_functions->componentEnvironment, _instanciateName, fmi2Error, "error",
			"fmi2EnterEventMode: %d", e);
		return fmi2Error;
	}
}
fmi2Status fmi2NewDiscreteStates(fmi2Component c,
	fmi2EventInfo* fmi2eventInfo) {
	try{
		_variant_t myVariantComponent = (_variant_t*)c;
		long res;
		long newDiscreteStatesNeeded = fmi2eventInfo->newDiscreteStatesNeeded;
		double nextEventTime = fmi2eventInfo->nextEventTime;
		long nextEventTimeDefined = fmi2eventInfo->nextEventTimeDefined;
		long nominalsOfContinuousStatesChanged = fmi2eventInfo->nominalsOfContinuousStatesChanged;
		long terminateSimulation = fmi2eventInfo->terminateSimulation;
		long valuesOfContinuousStatesChanged = fmi2eventInfo->valuesOfContinuousStatesChanged;

		pFMUManage->NewDiscreteStates(myVariantComponent,
			&newDiscreteStatesNeeded,
			&terminateSimulation,
			&nominalsOfContinuousStatesChanged,
			&valuesOfContinuousStatesChanged,
			&nextEventTimeDefined,
			&nextEventTime,
			&res);
				
		fmi2eventInfo->newDiscreteStatesNeeded = newDiscreteStatesNeeded;
		fmi2eventInfo->nextEventTime = nextEventTime;
		fmi2eventInfo->nextEventTimeDefined = nextEventTimeDefined;
		fmi2eventInfo->nominalsOfContinuousStatesChanged = nominalsOfContinuousStatesChanged;
		fmi2eventInfo->terminateSimulation = terminateSimulation;
		fmi2eventInfo->valuesOfContinuousStatesChanged = valuesOfContinuousStatesChanged;
		
		printf("fmi2eventInfo->newDiscreteStatesNeeded %d", fmi2eventInfo->newDiscreteStatesNeeded);

		return longTofmi2Status(res);
	}
	catch (int e){
		_functions->logger(_functions->componentEnvironment, _instanciateName, fmi2Error, "error",
			"fmi2NewDiscreteStates: %d", e);
		return fmi2Error;
	}
}
fmi2Status fmi2EnterContinuousTimeMode(fmi2Component c){
	try{
		_variant_t myVariantComponent = (_variant_t*)c;
		long res;
		pFMUManage->EnterContinuousTimeMode(myVariantComponent, &res);
		return longTofmi2Status(res);
	}
	catch (int e){
		_functions->logger(_functions->componentEnvironment, _instanciateName, fmi2Error, "error",
			"fmi2EnterContinuousTimeMode: %d", e);
		return fmi2Error;
	}
}
fmi2Status fmi2CompletedIntegratorStep(fmi2Component c,
	fmi2Boolean noSetFMUStatePriorToCurrentPoint,
	fmi2Boolean* enterEventMode,
	fmi2Boolean* terminateSimulation){
	try{
		_variant_t myVariantComponent = (_variant_t*)c;
		long res;
		long _enterEventMode =  *enterEventMode;
		long _terminateSimulation = *terminateSimulation;
		long  _noSetFMUStatePriorToCurrentPoint = noSetFMUStatePriorToCurrentPoint;
		pFMUManage->CompletedIntegratorStep(myVariantComponent, _noSetFMUStatePriorToCurrentPoint, &_enterEventMode, &_terminateSimulation, &res);
		*enterEventMode = _enterEventMode;
		*terminateSimulation = _terminateSimulation;
		noSetFMUStatePriorToCurrentPoint = _noSetFMUStatePriorToCurrentPoint;
		return longTofmi2Status(res);
	}
	catch (int e){
		_functions->logger(_functions->componentEnvironment, _instanciateName, fmi2Error, "error",
			"fmi2CompletedIntegratorStep: %d", e);
		return fmi2Error;
	}
}

/* Providing independent variables and re-initialization of caching */
fmi2Status fmi2SetTime(fmi2Component c, fmi2Real time){
	try{
		_variant_t myVariantComponent = (_variant_t*)c;
		long res;
		pFMUManage->SetTime(myVariantComponent, time, &res);
		return longTofmi2Status(res);
	}
	catch (int e){
		_functions->logger(_functions->componentEnvironment, _instanciateName, fmi2Error, "error",
			"fmi2SetTime: %d", e);
		return fmi2Error;
	}
}
fmi2Status fmi2SetContinuousStates(fmi2Component c, const fmi2Real x[], size_t nx) 
{
	try{
		_variant_t myVariantComponent = (_variant_t*)c;
		long res;
		
		// Value Double
		CComSafeArray<DOUBLE> ValeursData(nx);
		SAFEARRAY* ValeursDataSafe = ArrayDoubleToSAFEARRAY(ValeursData, x, nx);
		// Appel
		pFMUManage->SetContinuousStates(myVariantComponent, ValeursDataSafe, nx , &res);

		return longTofmi2Status(res);
	}
	catch (int e){
		_functions->logger(_functions->componentEnvironment, _instanciateName, fmi2Error, "error",
			"fmi2SetContinuousStates: %d", e);
		return fmi2Error;
	}
}

/* Evaluation of the model equations */
fmi2Status fmi2GetDerivatives(fmi2Component c, fmi2Real derivatives[], size_t nx)
{
	try{
		_variant_t myVariantComponent = (_variant_t*)c;
		long res;

		// Value Double
		CComSafeArray<DOUBLE> ValeursData(nx);
		SAFEARRAY* ValeursDataSafe = ArrayDoubleToSAFEARRAY(ValeursData, derivatives, nx);
		// Appel
		pFMUManage->GetDerivatives(myVariantComponent, &ValeursDataSafe, nx, &res);
		//affecte les résultats
		ValeursData.Attach(ValeursDataSafe);
		SAFEARRAYToArrayDouble(ValeursData, derivatives, nx);


		return longTofmi2Status(res);
	}
	catch (int e){
		_functions->logger(_functions->componentEnvironment, _instanciateName, fmi2Error, "error",
			"fmi2GetDerivatives: %d", e);
		return fmi2Error;
	}
}
fmi2Status fmi2GetEventIndicators(fmi2Component c, fmi2Real eventIndicators[], size_t ni)
{
	try{
		_variant_t myVariantComponent = (_variant_t*)c;
		long res;

		// Value Double
		CComSafeArray<DOUBLE> ValeursData(ni);
		SAFEARRAY* ValeursDataSafe = ArrayDoubleToSAFEARRAY(ValeursData, eventIndicators, ni);
		// Appel
		pFMUManage->GetEventIndicators(myVariantComponent, &ValeursDataSafe, ni, &res);
		//affecte les résultats
		ValeursData.Attach(ValeursDataSafe);
		SAFEARRAYToArrayDouble(ValeursData, eventIndicators, ni);

		return longTofmi2Status(res);
	}
	catch (int e){
		_functions->logger(_functions->componentEnvironment, _instanciateName, fmi2Error, "error",
			"fmi2GetEventIndicators: %d", e);
		return fmi2Error;
	}
}
fmi2Status fmi2GetContinuousStates(fmi2Component c, fmi2Real x[], size_t nx)
{
	try{
		_variant_t myVariantComponent = (_variant_t*)c;
		long res;

		// Value Double
		CComSafeArray<DOUBLE> ValeursData(nx);
		SAFEARRAY* ValeursDataSafe = ArrayDoubleToSAFEARRAY(ValeursData, x, nx);
		// Appel
		pFMUManage->GetContinuousStates(myVariantComponent, &ValeursDataSafe, nx, &res);
		//affecte les résultats
		ValeursData.Attach(ValeursDataSafe);
		SAFEARRAYToArrayDouble(ValeursData, x, nx);

		return longTofmi2Status(res);
	}
	catch (int e){
		_functions->logger(_functions->componentEnvironment, _instanciateName, fmi2Error, "error",
			"fmi2GetContinuousStates: %d", e);
		return fmi2Error;
	}
}
fmi2Status fmi2GetNominalsOfContinuousStates(fmi2Component c, fmi2Real x_nominal[], size_t nx){
	try{
		_variant_t myVariantComponent = (_variant_t*)c;
		long res;

		// Value Double
		CComSafeArray<DOUBLE> ValeursData(nx);
		SAFEARRAY* ValeursDataSafe = ArrayDoubleToSAFEARRAY(ValeursData, x_nominal, nx);
		// Appel
		pFMUManage->GetNominalsOfContinuousStates(myVariantComponent, &ValeursDataSafe, nx, &res);
		//affecte les résultats
		ValeursData.Attach(ValeursDataSafe);
		SAFEARRAYToArrayDouble(ValeursData, x_nominal, nx);

		return longTofmi2Status(res);
	}
	catch (int e){
		_functions->logger(_functions->componentEnvironment, _instanciateName, fmi2Error, "error",
			"fmi2GetNominalsOfContinuousStates: %d", e);
		return fmi2Error;
	}
}


///***************************************************
//  Functions for FMI2 for Co-Simulation
//****************************************************/

/* Simulating the slave */
fmi2Status fmi2SetRealInputDerivatives(fmi2Component c,
	const fmi2ValueReference vr[], size_t nvr,
	const fmi2Integer order[],
	const fmi2Real value[])
{
	
	try{
		_variant_t myVariantComponent = (_variant_t*)c;
		long res;
		// ref
		CComSafeArray<long> ValeursRef(nvr);
		SAFEARRAY* ValeursRefSafe = ArrayIntToSAFEARRAY(ValeursRef, vr, nvr);
		// order
		CComSafeArray<long> ValeursOrder(nvr);
		SAFEARRAY* ValeursOrderSafe = ArrayIntToSAFEARRAY(ValeursRef, order, nvr);
		// Value Double
		CComSafeArray<DOUBLE> ValeursData(nvr);
		SAFEARRAY* ValeursDataSafe = ArrayDoubleToSAFEARRAY(ValeursData, value, nvr);
		// Appel
		pFMUManage->SetRealInputDerivatives(myVariantComponent, ValeursRefSafe, ValeursOrderSafe, ValeursDataSafe, &res);

		return longTofmi2Status(res);
	}
	catch (int e){
		_functions->logger(_functions->componentEnvironment, _instanciateName, fmi2Error, "error",
			"fmi2SetRealInputDerivatives: %d", e);
		return fmi2Error;
	}
}
fmi2Status fmi2GetRealOutputDerivatives(fmi2Component c,
	const fmi2ValueReference vr[], size_t nvr,
	const fmi2Integer order[],
	fmi2Real value[]){
	try{
		_variant_t myVariantComponent = (_variant_t*)c;
		long res;
		// ref
		CComSafeArray<long> ValeursRef(nvr);
		SAFEARRAY* ValeursRefSafe = ArrayIntToSAFEARRAY(ValeursRef, vr, nvr);
		// order
		CComSafeArray<long> ValeursOrder(nvr);
		SAFEARRAY* ValeursOrderSafe = ArrayIntToSAFEARRAY(ValeursRef, order, nvr);
		// Value Double
		CComSafeArray<DOUBLE> ValeursData(nvr);
		SAFEARRAY* ValeursDataSafe = ArrayDoubleToSAFEARRAY(ValeursData, value, nvr);
		// Appel
		pFMUManage->GetRealOutputDerivatives(myVariantComponent, ValeursRefSafe, ValeursOrderSafe, &ValeursDataSafe, &res);
		//affecte les résultats
		ValeursData.Attach(ValeursDataSafe);
		SAFEARRAYToArrayDouble(ValeursData, value, nvr);


		return longTofmi2Status(res);
	}
	catch (int e){
		_functions->logger(_functions->componentEnvironment, _instanciateName, fmi2Error, "error",
			"fmi2GetRealOutputDerivatives: %d", e);
		return fmi2Error;
	}
}

fmi2Status fmi2DoStep(fmi2Component c,
	fmi2Real currentCommunicationPoint,
	fmi2Real communicationStepSize,
	fmi2Boolean noSetFMUStatePriorToCurrentPoint)
{
	try{
		_variant_t myVariantComponent = (_variant_t*)c;
		long res;
		pFMUManage->DoStep(myVariantComponent, currentCommunicationPoint, communicationStepSize, noSetFMUStatePriorToCurrentPoint, &res);
		return longTofmi2Status(res);
	}
	catch (int e){
		_functions->logger(_functions->componentEnvironment, _instanciateName, fmi2Error, "error",
			"fmi2DoStep: %d", e);
		return fmi2Error;
	}
}

fmi2Status fmi2CancelStep(fmi2Component c)
{
	try{
		_variant_t myVariantComponent = (_variant_t*)c;
		long res;
		pFMUManage->CancelStep(myVariantComponent, &res);
		return longTofmi2Status(res);
	}
	catch (int e){
		_functions->logger(_functions->componentEnvironment, _instanciateName, fmi2Error, "error",
			"CancelStep: %d", e);
		return fmi2Error;
	}
}

/* Inquire slave status */
fmi2Status fmi2GetStatus(fmi2Component c, const fmi2StatusKind s,
	fmi2Status* value){
	
	try{
		_variant_t myVariantComponent = (_variant_t*)c;
		long res;
		long _value = *value;
		pFMUManage->GetStatus(myVariantComponent, fmi2StatusKindTolong(s), &_value, &res);
		*value = longTofmi2Status(_value);
		return longTofmi2Status(res);
	}
	catch (int e){
		_functions->logger(_functions->componentEnvironment, _instanciateName, fmi2Error, "error",
			"fmi2GetStatus: %d", e);
		return fmi2Error;
	}
}
fmi2Status fmi2GetRealStatus(fmi2Component c, const fmi2StatusKind s,
	fmi2Real* value){
	try{
		_variant_t myVariantComponent = (_variant_t*)c;
		long res;
		double _value = *value;
		pFMUManage->GetRealStatus(myVariantComponent, fmi2StatusKindTolong(s), &_value, &res);
		*value = _value;
		return longTofmi2Status(res);
	}
	catch (int e){
		_functions->logger(_functions->componentEnvironment, _instanciateName, fmi2Error, "error",
			"fmi2GetRealStatus: %d", e);
		return fmi2Error;
	}
}
fmi2Status fmi2GetIntegerStatus(fmi2Component c, const fmi2StatusKind s,
	fmi2Integer* value){	
	try{
		_variant_t myVariantComponent = (_variant_t*)c;
		long res;
		long _value = *value;
		pFMUManage->GetIntegerStatus(myVariantComponent, fmi2StatusKindTolong(s), &_value, &res);
		*value = _value;
		return longTofmi2Status(res);
	}
	catch (int e){
		_functions->logger(_functions->componentEnvironment, _instanciateName, fmi2Error, "error",
			"fmi2GetIntegerStatus: %d", e);
		return fmi2Error;
	}
}
fmi2Status fmi2GetBooleanStatus(fmi2Component c, const fmi2StatusKind s,
	fmi2Boolean* value){
	try{
		_variant_t myVariantComponent = (_variant_t*)c;
		long res;
		long _value = *value;
		pFMUManage->GetBooleanStatus(myVariantComponent, fmi2StatusKindTolong(s), &_value, &res);
		*value =  _value;
		return longTofmi2Status(res);
	}
	catch (int e){
		_functions->logger(_functions->componentEnvironment, _instanciateName, fmi2Error, "error",
			"fmi2GetBooleanStatus: %d", e);
		return fmi2Error;
	}
}
fmi2Status fmi2GetStringStatus(fmi2Component c, const fmi2StatusKind s,
	fmi2String* value){

	return fmi2OK;
}
