using IdentityModel.OidcClient;
using SQLite;

namespace HelloOIDC.Repo {

    [Table("OpenIdSessions")]
    public class OpenIdSession
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string StartUrl { get; set; }

        public string State { get; set; }

        public string CodeVerifier { get; set; }

        public string RedirectUri { get; set; }

        public DateTime Time { get; set; }
    }

    public class OIDCRepo {
        public OIDCRepo()
        {
            using (var _db = new SQLiteConnection("db.db")) {
                _db.CreateTable<OpenIdSession>();
            }
        }

        public void SetState(OpenIdSession state) {
            using (var _db = GetDb()) {
                _db.Insert(state);
            }
        }

        public OpenIdSession GetState(string state) {
            using (var _db = GetDb()) {
                var results = _db.Query<OpenIdSession>("select * from OpenIdSessions where state = ?", state);
                return results.First();
            }
        }

        public void DeleteState(string state) {
            using (var _db = GetDb()) {
                var results = _db.Execute("delete from OpenIdSessions where state = ?", state);
            }

        }

        private static SQLiteConnection GetDb() {
            return new SQLiteConnection("db.db");
        }

        public void Cleanup() {
            using (var _db = GetDb()) {
                var results = _db.Execute("delete from OpenIdSessions where Time < ?", DateTime.UtcNow.AddMinutes(-1));
            }
        }
    }
}
