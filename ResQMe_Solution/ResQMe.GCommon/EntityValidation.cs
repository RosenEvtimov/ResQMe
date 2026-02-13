namespace ResQMe.GCommon
{
    public static class EntityValidation
    {
        /* Animal */
        public const int MinAnimalNameLength = 3;
        public const int MaxAnimalNameLength = 50;

        public const int MinAnimalAge = 0;
        public const int MaxAnimalAge = 40;

        public const int MinAnimalDescriptionLength = 20;
        public const int MaxAnimalDescriptionLength = 500;

        /* Species */
        public const int MinSpeciesNameLength = 3;
        public const int MaxSpeciesNameLength = 30;

        /* Breed */
        public const int MinBreedNameLength = 3;
        public const int MaxBreedNameLength = 50;

        /* Shelter */
        public const int MinShelterNameLength = 3;
        public const int MaxShelterNameLength = 50;

        public const int MinShelterCityLength = 1;
        public const int MaxShelterCityLength = 60;

        public const int MinShelterAddressLength = 5;
        public const int MaxShelterAddressLength = 150;

        public const int MinShelterDescriptionLength = 20;
        public const int MaxShelterDescriptionLength = 750;

        public const int MinShelterPhoneLength = 7;
        public const int MaxShelterPhoneLength = 20;

        public const int MinShelterEmailLength = 3;
        public const int MaxShelterEmailLength = 40;

        /* AdoptionRequest */
        public const int MinAdoptionRequestFirstNameLength = 1;
        public const int MaxAdoptionRequestFirstNameLength = 40;

        public const int MinAdoptionRequestLastNameLength = 1;
        public const int MaxAdoptionRequestLastNameLength = 40;

        public const int MinAdoptionRequestEmailLength = 3;
        public const int MaxAdoptionRequestEmailLength = 40;

        public const int MinAdoptionRequestPhoneLength = 7;
        public const int MaxAdoptionRequestPhoneLength = 20;

        public const int MaxAdoptionRequestMessageLength = 500;
    }
}