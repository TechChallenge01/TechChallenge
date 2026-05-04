namespace Domain.ValueObjects
{
    public class Email
    {
        protected Email() { }
        public Email(string email)
        {
            if (string.IsNullOrEmpty(email))
                throw new ArgumentException("O email não pode ser nulo ou vazio.");

            if (!ValidaEmail(email))
                throw new ArgumentException("Formato do email inválido.");

            EnderecoEmail = email.ToLower().Trim(); ;
        }

        public string EnderecoEmail { get; private set; }

        private bool ValidaEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        public override bool Equals(object? obj)
        {
            if (obj is Email outroEmail)
            {
                return EnderecoEmail == outroEmail.EnderecoEmail;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return EnderecoEmail.GetHashCode();
        }
    }
}
