using JARS_DAL.Models;
using JARS_DAL.DAO;
namespace JARS_DAL.Repository;

public class NoteRepository : INoteRepository
{
    public Task<IEnumerable<Note>> GetNotes() => NoteManagement.Instance.GetNotes();
    public Task<Note?> GetNote(int id) => NoteManagement.Instance.GetNote(id);
    public Task Delete(Note note) => NoteManagement.Instance.Delete(note);
    public Task Add(Note note) => NoteManagement.Instance.Add(note);
    public Task Update(Note note) => NoteManagement.Instance.Update(note);
}