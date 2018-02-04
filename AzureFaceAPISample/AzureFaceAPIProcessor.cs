//|---------------------------------------------------------------|
//|                     AZURE FACE API CLIENT                     |
//|---------------------------------------------------------------|
//|                     Developed by Wonde Tadesse                |
//|                        Copyright ©2018 - Present              |
//|---------------------------------------------------------------|
//|                     AZURE FACE API CLIENT                     |
//|---------------------------------------------------------------|

using Microsoft.ProjectOxford.Face;
using Microsoft.ProjectOxford.Face.Contract;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AzureFaceAPISample.Entities;

namespace AzureFaceAPIWPFClient
{
    /// <summary>
    /// Azure Face API processor
    /// </summary>
    public class AzureFaceAPIProcessor
    {
        #region Private Variable 

        private IFaceServiceClient faceServiceClient;

        #endregion

        #region Constructor 

        /// <summary>
        /// Azure Face API processor
        /// </summary>
        /// <param name="subscriptionKey">SubscriptionKey value</param>
        /// <param name="url">Url value</param>
        public AzureFaceAPIProcessor(string subscriptionKey, string url)
        {
            faceServiceClient = new FaceServiceClient(subscriptionKey, url);
        }

        #endregion

        #region Public Methods 

        /// <summary>
        /// Detect Faces 
        /// </summary>
        /// <param name="stream">Stream value</param>
        /// <returns>List of face object</returns>
        public async Task<List<Face>> DetectFaces(Stream stream)
        {
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }
            IEnumerable<FaceAttributeType> faceAttributes =
                new FaceAttributeType[]
                {
                    FaceAttributeType.Accessories,
                    FaceAttributeType.Blur,
                    FaceAttributeType.HeadPose,
                    FaceAttributeType.Exposure,
                    FaceAttributeType.FacialHair,
                    FaceAttributeType.Glasses,
                    FaceAttributeType.Gender,
                    FaceAttributeType.Age,
                    FaceAttributeType.Smile,
                    FaceAttributeType.Emotion,
                    FaceAttributeType.Makeup,
                    FaceAttributeType.Hair,
                    FaceAttributeType.Occlusion,
                    FaceAttributeType.Noise
                };

            try
            {
                using (stream)
                {
                    Face[] faces = await faceServiceClient.DetectAsync(stream, returnFaceId: true, returnFaceLandmarks: false, returnFaceAttributes: faceAttributes);
                    return faces.ToList();
                }
            }
            catch (FaceAPIException faceAPIException)
            {
                throw faceAPIException;
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        /// <summary>
        /// Train Faces
        /// </summary>
        /// <param name="trainFace">TrainFace object</param>
        /// <returns>true/false</returns>
        public async Task<bool> TrainFaces(TrainFace trainFace)
        {
            if (trainFace == null)
            {
                throw new ArgumentNullException("trainFace");
            }
            PersonGroup personGroup = null;
            try
            {
                personGroup = await faceServiceClient.GetPersonGroupAsync(trainFace.PersonGroupID);
            }
            catch (Exception)
            {
                personGroup = null;
            }
            if (personGroup == null)
            {
                try
                {
                    await faceServiceClient.CreatePersonGroupAsync(trainFace.PersonGroupID,
                        trainFace.PersonGroupName);
                    await faceServiceClient.GetPersonGroupAsync(trainFace.PersonGroupID);
                }
                catch (Exception exception)
                {
                    throw exception;
                }
            }
            try
            {
                CreatePersonResult person = await faceServiceClient.CreatePersonAsync(
                    trainFace.PersonGroupID,
                    trainFace.PersonName
                );

                foreach (string imagePath in Directory.GetFiles(trainFace.ImageDirectoryPath, trainFace.ImageFilterType))
                {
                    FaceRectangle faceRectangle = null;
                    using (Stream stream = File.OpenRead(imagePath))
                    {
                        Face[] faces = await faceServiceClient.DetectAsync(stream);
                        if (faces != null && faces.Count() == 1)
                        {
                            faceRectangle = faces[0].FaceRectangle;
                        }
                    }
                    if (faceRectangle != null)
                    {
                        using (Stream stream = File.OpenRead(imagePath))
                        {
                            // Detect faces in the image and add to Anna
                            await faceServiceClient.AddPersonFaceAsync(
                                trainFace.PersonGroupID, person.PersonId, stream, null, faceRectangle);
                        }
                    }
                }

                await faceServiceClient.TrainPersonGroupAsync(trainFace.PersonGroupID);

                TrainingStatus trainingStatus = null;
                while (true)
                {
                    trainingStatus = await faceServiceClient.
                        GetPersonGroupTrainingStatusAsync(trainFace.PersonGroupID);

                    if (trainingStatus.Status != Status.Running)
                    {
                        break;
                    }
                    await Task.Delay(2000);
                }
            }
            catch (FaceAPIException faceAPIException)
            {
                throw faceAPIException;
            }
            catch (Exception exception)
            {
                throw exception;
            }
            return true;
        }

        /// <summary>
        /// Get Gender Chart
        /// </summary>
        /// <param name="faces">List of Face object</param>
        /// <returns>GenderChart object</returns>
        public GenderChart GetGenderChart(List<Face> faces)
        {
            if (faces == null || (faces != null && faces.Count() == 0))
                throw new ArgumentNullException("faces");
            GenderChart genderChart = new GenderChart();
            var noOfMales = faces.ToList().FindAll(f => f.FaceAttributes.Gender.ToLower().Equals("male")).ToList().Count;
            var noOfFemales = faces.ToList().FindAll(f => f.FaceAttributes.Gender.ToLower().Equals("female")).ToList().Count;
            genderChart.NoOfMale = noOfMales.ToString();
            genderChart.NoOfFemale = noOfFemales.ToString();
            return genderChart;
        }

        /// <summary>
        /// AgeGroup Chart
        /// </summary>
        /// <param name="faces">List of Face object</param>
        /// <returns></returns>
        public AgeGroupChart AgeGroupChart(List<Face> faces)
        {
            if (faces == null || (faces != null && faces.Count() == 0))
                throw new ArgumentNullException("faces");
            AgeGroupChart ageGroupChart = new AgeGroupChart();
            var noOfAdolescence = faces.ToList().FindAll(f => f.FaceAttributes.Age < 18).ToList().Count;
            var noOfYoungAdult = faces.ToList().FindAll(f => f.FaceAttributes.Age >= 18 && f.FaceAttributes.Age <= 35).ToList().Count;
            var noOfMiddleAgeAdult = faces.ToList().FindAll(f => f.FaceAttributes.Age > 35 && f.FaceAttributes.Age <= 55).ToList().Count;
            var noOfOldAdult = faces.ToList().FindAll(f => f.FaceAttributes.Age > 55).ToList().Count;
            ageGroupChart.NoOfAdolescence = noOfAdolescence.ToString();
            ageGroupChart.NoOfYoungAdult = noOfYoungAdult.ToString();
            ageGroupChart.NoOfMiddleAgedAdult = noOfMiddleAgeAdult.ToString();
            ageGroupChart.NoOfOldAdult = noOfOldAdult.ToString();
            return ageGroupChart;
        }

        /// <summary>
        /// Identify Face
        /// </summary>
        /// <param name="personGroupId">PersonGroupID value</param>
        /// <param name="stream">Stream value</param>
        /// <returns></returns>
        public async Task<IdentifyFace> IdentifyFace(string personGroupId, Stream stream)
        {
            IdentifyFace identifyFace = new IdentifyFace();
            if (string.IsNullOrWhiteSpace(personGroupId))
            {
                throw new ArgumentNullException("personGroupId");
            }
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }
            try
            {
                using (stream)
                {
                    var faces = await DetectFaces(stream);
                    var faceIds = faces.Select(face => face.FaceId).ToArray();

                    var results = await faceServiceClient.IdentifyAsync(personGroupId, faceIds);
                    foreach (var identifyResult in results)
                    {
                        if (identifyResult.Candidates.Length != 0)
                        {
                            var candidateId = identifyResult.Candidates[0].PersonId;
                            var person = await faceServiceClient.GetPersonAsync(personGroupId, candidateId);
                            identifyFace.IsFound = true;
                            identifyFace.PersonName = person.Name;
                            break;
                        }
                    }
                }
            }
            catch (FaceAPIException faceAPIException)
            {
                throw faceAPIException;
            }
            catch (Exception exception)
            {
                throw exception;
            }
            return identifyFace;
        }

        #endregion
    }
}
