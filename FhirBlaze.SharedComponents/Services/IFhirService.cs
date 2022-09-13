using Hl7.Fhir.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FhirBlaze.SharedComponents.Services
{
    public interface IFhirService
    {
        /// <summary>
        /// Get a FHIR resource by its resourceId.
        /// </summary>
        /// <typeparam name="TResource"></typeparam>
        /// <param name="resourceId"></param>
        /// <returns></returns>
        Task<TResource> GetResourceByIdAsync<TResource>(string resourceId) where TResource : Resource, new();

        #region Patient
        /// <summary>
        /// Create a new <see cref="Patient"/>.
        /// </summary>
        /// <param name="patient"></param>
        /// <returns></returns>
        Task<Patient> CreatePatientAsync(Patient patient);
        /// <summary>
        /// Get a list of all patients.
        /// </summary>
        /// <returns></returns>
        Task<IList<Patient>> GetPatientsAsync();
        /// <summary>
        /// Get a list of all patients where lastModified date is between startLastModified and endLastModified.
        /// </summary>
        /// <param name="startLastModified"></param>
        /// <param name="endLastModified"></param>
        /// <returns></returns>
        Task<IList<Patient>> GetPatientsAsync(DateTime startLastModified, DateTime endLastModified);
        /// <summary>
        /// Get a list of patients using specified <see cref="PatientFilters"/> values.
        /// </summary>
        /// <param name="patientFilters"></param>
        /// <returns></returns>
        Task<IList<Patient>> GetPatientsAsync(PatientFilters patientFilters);
        /// <summary>
        /// Update a <see cref="Patient"/>.
        /// </summary>
        /// <param name="patientId"></param>
        /// <param name="patient"></param>
        /// <returns></returns>
        Task<Patient> UpdatePatientAsync(string patientId, Patient patient);
        /// <summary>
        /// Return a count of all patient records.
        /// </summary>
        /// <returns></returns>
        Task<int> GetPatientCountAsync();
        /// <summary>
        /// Return a list of patients using search criteria matching specified <see cref="Patient"/> properties.
        /// </summary>
        /// <param name="patient"><see cref="Patient"/></param>
        /// <returns></returns>
        Task<IList<Patient>> SearchPatient(Patient patient);
        #endregion

        #region Questionnaire
        Task<QuestionnaireResponse> SaveQuestionnaireResponseAsync(QuestionnaireResponse qResponse);
        Task<IList<Questionnaire>> GetQuestionnairesAsync();
        Task<Questionnaire> GetQuestionnaireByIdAsync(string id);
        Task<Questionnaire> CreateQuestionnaireAsync(Questionnaire questionnaire);
        Task<QuestionnaireResponse> GetQuestionnaireResponseByIdAsync(string id);
        Task<IList<QuestionnaireResponse>> GetQuestionnaireResponsesByQuestionnaireIdAsync(string questionnaireId);
        Task<Questionnaire> UpdateQuestionnaireAsync(Questionnaire questionnaire);

        Task<IList<Questionnaire>> SearchQuestionnaire(string title);
        #endregion

        #region Practitioners
        Task<IList<Practitioner>> GetPractitionersAsync();

        Task<int> GetPractitionerCountAsync();

        Task<IList<Practitioner>> SearchPractitioner(IDictionary<string, string> searchParameters);

        Task<Practitioner> CreatePractitionersAsync(Practitioner practitioner);

        Task<Practitioner> UpdatePractitionerAsync(string practitionerId, Practitioner practitioner);
        #endregion
    }
}